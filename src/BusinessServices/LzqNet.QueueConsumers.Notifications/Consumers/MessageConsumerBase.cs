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
    protected string MainQueueName => $"{TopicName}_queue";
    protected string RetryQueueName => $"{TopicName}_retry_queue";
    protected string DeadLetterQueueName => $"{TopicName}_dead_letter_queue";

    // 交换机名称
    protected string MainExchangeName => TopicName;
    protected string RetryExchangeName => $"{TopicName}_retry_exchange";
    protected string DeadLetterExchangeName => $"{TopicName}_dead_letter_exchange";

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

            // 启动死信队列消费者
            await StartDeadLetterConsumerAsync(stoppingToken);

            // 启动重试队列消费者
            await StartRetryConsumerAsync(stoppingToken);
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

    private async Task StartRetryConsumerAsync(CancellationToken stoppingToken)
    {
        try
        {
            var retryConsumer = new AsyncEventingBasicConsumer(_channel);

            retryConsumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                await ProcessRetryMessageAsync(eventArgs, stoppingToken);
            };

            await _channel!.BasicConsumeAsync(
                queue: RetryQueueName,
                autoAck: false,
                consumer: retryConsumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("重试队列消费者已启动，队列：{QueueName}", RetryQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动重试队列消费者失败");
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

    private async Task ProcessRetryMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        string messageId = eventArgs.BasicProperties.MessageId ?? Guid.NewGuid().ToString();

        try
        {
            var body = eventArgs.Body.ToArray();
            var retryCount = GetRetryCount(eventArgs.BasicProperties.Headers);
            var newRetryCount = retryCount + 1;

            _logger.LogInformation("处理重试消息，消息ID: {MessageId}，当前重试次数: {RetryCount}",
                messageId, retryCount);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = messageId,
                Headers = new Dictionary<string, object>()
            };

            if (eventArgs.BasicProperties.Headers != null)
            {
                foreach (var header in eventArgs.BasicProperties.Headers)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }

            properties.Headers["x-retry-count"] = newRetryCount;

            await _channel!.BasicPublishAsync(
                exchange: MainExchangeName,
                routingKey: $"{TopicName}_routingKey",
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: stoppingToken);

            await _channel!.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            _logger.LogInformation("重试消息已重新发布到主队列，消息ID: {MessageId}，重试次数: {RetryCount}",
                messageId, newRetryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理重试消息失败，消息ID: {MessageId}", messageId);
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

        if (retryCount >= MaxRetryCount)
        {
            _logger.LogWarning("消息已达到最大重试次数({MaxRetryCount})，转入死信队列，消息ID: {MessageId}，错误: {ErrorMessage}",
                MaxRetryCount, messageId, errorMessage);

            await _channel!.BasicRejectAsync(
                deliveryTag: eventArgs.DeliveryTag,
                requeue: false,
                cancellationToken: CancellationToken.None);
        }
        else
        {
            await SendToRetryQueueAsync(eventArgs, retryCount);
        }
    }

    private async Task SendToRetryQueueAsync(BasicDeliverEventArgs eventArgs, int currentRetryCount)
    {
        string messageId = eventArgs.BasicProperties.MessageId ?? Guid.NewGuid().ToString();

        try
        {
            var newRetryCount = currentRetryCount + 1;

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = messageId,
                Headers = new Dictionary<string, object>()
            };

            if (eventArgs.BasicProperties.Headers != null)
            {
                foreach (var header in eventArgs.BasicProperties.Headers)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }

            properties.Headers["x-retry-count"] = newRetryCount;

            await _channel!.BasicPublishAsync(
                exchange: RetryExchangeName,
                routingKey: $"{TopicName}_retry_routingKey",
                mandatory: false,
                basicProperties: properties,
                body: eventArgs.Body.ToArray(),
                cancellationToken: CancellationToken.None);

            await _channel!.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: CancellationToken.None);

            _logger.LogInformation("消息已发送到重试队列，消息ID: {MessageId}，新的重试次数: {RetryCount}",
                messageId, newRetryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送消息到重试队列失败，消息ID: {MessageId}", messageId);
            await _channel!.BasicRejectAsync(
                deliveryTag: eventArgs.DeliveryTag,
                requeue: false,
                cancellationToken: CancellationToken.None);
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