using Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace LzqNet.QueueConsumers.Notifications.Consumers;

/// <summary>
/// 通用消息消费者基类
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
public abstract class MessageConsumerBase<TMessage> : BackgroundService where TMessage : class
{
    private readonly RabbitMqOptions _options;
    protected readonly ILogger<MessageConsumerBase<TMessage>> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    // 配置参数
    protected abstract string TopicName { get; }
    // 队列名称
    protected string MainExchangeName => TopicName;
    protected string MainQueueName => $"{TopicName}_queue";
    protected string MainRoutingKey => $"{TopicName}_routingKey";

    protected string DeadLetterExchangeName => $"{TopicName}_dead_letter_exchange";
    protected string DeadLetterQueueName => $"{TopicName}_dead_letter_queue";
    protected string DeadLetterRoutingKey => $"{TopicName}_dead_letter_routingKey";

    // 重试配置
    protected virtual int MaxRetryCount => 3;
    protected virtual int RetryDelayMs => 30000;

    protected MessageConsumerBase(
        IOptions<RabbitMqOptions> options,
        ILogger<MessageConsumerBase<TMessage>> logger)
    {
        _options = options.Value;
        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
        };
    }

    /// <summary>
    /// 处理消息的业务逻辑（由子类实现）
    /// </summary>
    protected abstract Task<bool> ProcessMessageAsync(TMessage message, string messageId, int retryCount, CancellationToken stoppingToken);

    /// <summary>
    /// 处理死信消息（可重写）
    /// </summary>
    protected virtual Task ProcessDeadLetterMessageAsync(string messageJson, string messageId, CancellationToken stoppingToken)
    {
        _logger.LogWarning("收到死信队列消息，消息ID: {MessageId}，内容：{Message}",
            messageId, messageJson);
        return Task.CompletedTask;
    }

    private async Task InitializeChannelAsync(CancellationToken stoppingToken)
    {
        try
        {
            // 创建连接
            _connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);

            // 连接事件处理
            _connection.ConnectionShutdownAsync += async (sender, args) =>
            {
                _logger.LogWarning("RabbitMQ连接关闭: {ReplyText}", args.ReplyText);
            };

            _connection.ConnectionBlockedAsync += async (sender, args) =>
            {
                _logger.LogWarning("RabbitMQ连接被阻塞: {Reason}", args.Reason);
            };

            _connection.ConnectionUnblockedAsync += async (sender, args) =>
            {
                _logger.LogInformation("RabbitMQ连接解除阻塞");
            };

            // 创建通道
            _channel = await _connection.CreateChannelAsync();
            await DeclareExchangeAndQueueAsync(_channel, stoppingToken);

            // 启动死信队列消费者
            await StartDeadLetterConsumerAsync(stoppingToken);

            // 通道事件处理
            _channel.CallbackExceptionAsync += async (sender, args) =>
            {
                _logger.LogError(args.Exception, "通道回调异常");
            };

            _channel.ChannelShutdownAsync += async (sender, args) =>
            {
                _logger.LogWarning("通道关闭: {ReplyText}", args.ReplyText);
            };

            // 设置QoS，公平分发
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            _logger.LogInformation("消息消费者初始化完成，Topic：{TopicName}，队列：{QueueName}",
                TopicName, MainQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化RabbitMQ通道失败");
            throw;
        }
    }

    private async Task StartDeadLetterConsumerAsync(CancellationToken stoppingToken)
    {
        try
        {
            var deadLetterConsumer = new AsyncEventingBasicConsumer(_channel);

            deadLetterConsumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                await ProcessDeadLetterInternalAsync(eventArgs, stoppingToken);
            };

            await _channel!.BasicConsumeAsync(
                queue: DeadLetterQueueName,
                autoAck: false,
                consumer: deadLetterConsumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("死信队列消费者已启动，队列：{QueueName}", DeadLetterQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动死信队列消费者失败");
        }
    }

    private async Task ProcessDeadLetterInternalAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        string messageId = eventArgs.BasicProperties.MessageId ?? Guid.NewGuid().ToString();

        try
        {
            var body = eventArgs.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            await ProcessDeadLetterMessageAsync(messageJson, messageId, stoppingToken);

            // 确认消息
            await _channel!.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理死信消息失败，消息ID: {MessageId}", messageId);
            // 死信消息处理失败也确认，避免无限循环
            await _channel!.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await InitializeChannelAsync(stoppingToken);

            if (_channel == null)
            {
                throw new InvalidOperationException("RabbitMQ通道未初始化");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                await ProcessMainMessageAsync(eventArgs, stoppingToken);
            };

            var consumerTag = await _channel.BasicConsumeAsync(
                queue: MainQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("开始消费消息，Topic：{TopicName}，队列：{QueueName}，消费者标签: {ConsumerTag}",
                TopicName, MainQueueName, consumerTag);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger?.LogInformation("消费者服务被取消");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "消费者服务执行失败");
            throw;
        }
        finally
        {
            await CleanupResourcesAsync();
        }
    }

    private async Task ProcessMainMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        string messageId = eventArgs.BasicProperties.MessageId ?? Guid.NewGuid().ToString();
        int retryCount = GetRetryCount(eventArgs.BasicProperties.Headers);

        try
        {
            _logger.LogDebug("开始处理消息，消息ID: {MessageId}，重试次数: {RetryCount}，Topic：{TopicName}",
                messageId, retryCount, TopicName);

            var body = eventArgs.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<TMessage>(messageJson);
            if (message == null)
            {
                _logger.LogError("消息反序列化失败，消息ID: {MessageId}", messageId);
                await HandleMessageFailureAsync(eventArgs, retryCount, "消息反序列化失败");
                return;
            }

            var success = await ProcessMessageAsync(message, messageId, retryCount, stoppingToken);

            if (success)
            {
                await _channel!.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("消息处理成功，消息ID: {MessageId}，重试次数: {RetryCount}",
                    messageId, retryCount);
            }
            else
            {
                await HandleMessageFailureAsync(eventArgs, retryCount, "业务处理失败");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("消息处理被取消，消息ID: {MessageId}", messageId);
            await _channel!.BasicNackAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true,
                cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息失败，消息ID: {MessageId}，重试次数: {RetryCount}",
                messageId, retryCount);
            await HandleMessageFailureAsync(eventArgs, retryCount, ex.Message);
        }
    }

    private async Task HandleMessageFailureAsync(BasicDeliverEventArgs eventArgs, int retryCount, string errorMessage)
    {
        string messageId = eventArgs.BasicProperties.MessageId ?? Guid.NewGuid().ToString();

        // 复制消息体和头信息
        var bodyCopy = eventArgs.Body.ToArray();
        var headersCopy = eventArgs.BasicProperties.Headers?.ToDictionary(kv => kv.Key, kv => kv.Value);

        _logger.LogInformation("处理消息失败，准备处理失败逻辑，消息ID: {MessageId}，当前重试次数: {RetryCount}，最大重试次数: {MaxRetryCount}，错误: {ErrorMessage}",
            messageId, retryCount, MaxRetryCount, errorMessage);

        try
        {
            if (retryCount >= MaxRetryCount)
            {
                _logger.LogWarning("消息已达到最大重试次数({MaxRetryCount})，转入死信队列，消息ID: {MessageId}，错误: {ErrorMessage}",
                    MaxRetryCount, messageId, errorMessage);

                await SendToDeadLetterQueueAsync(bodyCopy, headersCopy, retryCount, messageId);
            }
            else
            {
                _logger.LogInformation("消息未达到最大重试次数，增加重试次数并重新发布到主队列，消息ID: {MessageId}，当前重试次数: {RetryCount}",
                    messageId, retryCount);
                await RetryMessageAsync(bodyCopy, headersCopy, retryCount, messageId);
            }
        }
        finally
        {
            await _channel!.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: CancellationToken.None);
        }
    }

    private async Task RetryMessageAsync(byte[] bodyCopy, Dictionary<string, object?>? headersCopy, int currentRetryCount, string messageId)
    {
        try
        {
            var newRetryCount = currentRetryCount + 1;

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                MessageId = messageId,
                Headers = new Dictionary<string, object>(),
            };

            // 复制原始头信息
            if (headersCopy != null)
            {
                foreach (var header in headersCopy)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }

            // 更新重试次数
            properties.Headers["x-retry-count"] = newRetryCount;

            // 3.关键：设置延时时间(单位：毫秒)
            var retryDelayMs = (int)(RetryDelayMs * Math.Pow(2, newRetryCount));
            properties.Headers["x-delay"] = retryDelayMs;

            await _channel!.BasicPublishAsync(
                exchange: MainExchangeName,
                routingKey: MainRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: bodyCopy,
                cancellationToken: CancellationToken.None);

            _logger.LogInformation("消息已进入延时交换机，ID: {MessageId}，将在 {Delay}ms 后重试", messageId, retryDelayMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新发布消息到主队列失败，消息ID: {MessageId}", messageId);
            // 重试发布失败，直接转到死信队列
            await SendToDeadLetterQueueAsync(bodyCopy, headersCopy, currentRetryCount, messageId);
        }
    }

    private async Task SendToDeadLetterQueueAsync(byte[] bodyCopy, Dictionary<string, object?>? headersCopy, int retryCount,string messageId)
    {
        try
        {
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                MessageId = messageId,
                Headers = new Dictionary<string, object>()
            };

            // 复制原始头信息
            if (headersCopy != null)
            {
                foreach (var header in headersCopy)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }

            // 记录最终的重试次数
            properties.Headers["x-final-retry-count"] = retryCount;
            properties.Headers["x-dead-letter-time"] = DateTime.UtcNow.ToString("o");

            await _channel!.BasicPublishAsync(
                exchange: DeadLetterExchangeName,
                routingKey: DeadLetterRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: bodyCopy,
                cancellationToken: CancellationToken.None);

            _logger.LogInformation("消息已发送到死信队列，消息ID: {MessageId}，最终重试次数: {RetryCount}",
                messageId, retryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送消息到死信队列失败，消息ID: {MessageId}", messageId);
            // 死信队列也失败，只能记录日志，消息会丢失
        }
    }

    private int GetRetryCount(IDictionary<string, object>? headers)
    {
        if (headers != null && headers.TryGetValue("x-retry-count", out var retryCountObj))
        {
            if (retryCountObj is int count)
                return count;
            if (retryCountObj is long longCount)
                return (int)longCount;
            if (retryCountObj is byte byteCount)
                return byteCount;
        }
        return 0;
    }

    private async Task DeclareExchangeAndQueueAsync(IChannel channel, CancellationToken stoppingToken)
    {
        // 1. 声明死信交换机
        await channel.ExchangeDeclareAsync(
            exchange: DeadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        // 2. 声明死信队列
        await channel.QueueDeclareAsync(
            queue: DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        // 3. 绑定死信队列到死信交换机
        await channel.QueueBindAsync(
            queue: DeadLetterQueueName,
            exchange: DeadLetterExchangeName,
            routingKey: DeadLetterRoutingKey,
            arguments: null,
            cancellationToken: stoppingToken);

        // 4. 修改主交换机声明：支持延时插件
        var exchangeArguments = new Dictionary<string, object>
        {
            { "x-delayed-type", "direct" } // 指定实际的路由类型
        };
        await channel.ExchangeDeclareAsync(
            exchange: MainExchangeName,
            type: "x-delayed-message", // 必须是这个固定类型
            durable: true,
            autoDelete: false,
            arguments: exchangeArguments,
            cancellationToken: stoppingToken);

        // 5. 声明主队列，设置队列最大长度
        var mainQueueArguments = new Dictionary<string, object>
        {
            // 队列最大长度
            ["x-max-length"] = 10000,
            // 队列溢出行为
            ["x-overflow"] = "reject-publish"
        };

        await channel.QueueDeclareAsync(
            queue: MainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArguments,
            cancellationToken: stoppingToken);

        // 6. 绑定主队列到主交换机
        await channel.QueueBindAsync(
            queue: MainQueueName,
            exchange: MainExchangeName,
            routingKey: MainRoutingKey,
            arguments: null,
            cancellationToken: stoppingToken);

        _logger?.LogDebug("交换机、队列声明完成，Topic: {TopicName}", TopicName);
    }

    private async Task CleanupResourcesAsync()
    {
        try
        {
            if (_channel?.IsOpen == true)
            {
                await _channel.CloseAsync();
            }

            if (_connection?.IsOpen == true)
            {
                await _connection.CloseAsync();
            }

            _channel?.Dispose();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ资源已清理");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理RabbitMQ资源时出错");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await CleanupResourcesAsync();
        await base.StopAsync(cancellationToken);
    }
}