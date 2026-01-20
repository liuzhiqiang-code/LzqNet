using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq;

/// <summary>
/// RabbitMq消息发布器
/// </summary>
public class Publisher : IPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<Publisher> _logger;
    private readonly ConnectionFactory _connectionFactory;

    // 长连接缓存
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    // 缓存声明队列、交换机、绑定关系
    private readonly ConcurrentDictionary<string, bool> _declaredTopics = new();

    public Publisher(IOptions<RabbitMqOptions> options, ILogger<Publisher> logger)
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
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
    }

    public async Task PublishAsync<T>(string topicName, T @event, IntegrationEventExpand? eventMessageExpand, CancellationToken stoppingToken = default)
    {
        try
        {
            var connection = await GetConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync();//创建通道
            await DeclareExchangeAndQueueAsync(channel, topicName, stoppingToken);//声明交换机、队列、绑定关系

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            // 配置消息属性
            var properties = new BasicProperties
            {
                Persistent = true,                            // 消息持久化
                ContentType = "application/json",             // JSON格式
                DeliveryMode = DeliveryModes.Persistent,      // 持久化投递
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                MessageId = Guid.NewGuid().ToString()         // 消息唯一标识
            };

            //// 注册确认事件处理器
            //channel.BasicAcksAsync += async (sender, args) =>
            //{
            //    // 成功确认
            //    Console.WriteLine($"Message {args.DeliveryTag} acknowledged");
            //    if (args.Multiple)
            //    {
            //        _logger.LogDebug($"All messages up to {args.DeliveryTag} acknowledged");
            //    }
            //};

            channel.BasicNacksAsync += async (sender, args) =>
            {
                // 否定确认（需要重试）
                _logger.LogError($"Message {args.DeliveryTag} nacked");
            };

            channel.BasicReturnAsync += async (sender, args) =>
            {
                // 消息无法路由（mandatory=true 时触发）
                _logger.LogError($"Message returned: {args.ReplyText}");
            };

            // 通道异常处理
            channel.CallbackExceptionAsync += async (sender, args) =>
            {
                _logger.LogError(args.Exception, "Channel callback exception");
            };

            channel.ChannelShutdownAsync += async (sender, args) =>
            {
                _logger.LogWarning($"Channel shutdown: {args.ReplyText}");
            };

            // 发布消息到交换机
            await channel.BasicPublishAsync(
                exchange: topicName,
                routingKey: topicName + "_routingKey",
                mandatory: true,  // 根据接口定义添加这个参数
                basicProperties: properties,
                body: body,
                cancellationToken: stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning("发布操作被用户取消, Topic: {Topic}", topicName);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "RabbitMQ发布失败, Topic: {Topic}, Error: {ErrorMessage}",
                topicName, ex.Message);
            throw new Exception($"RabbitMQ发布失败: {ex.Message}", ex);
        }
    }

    public async Task BulkPublishAsync<T>(string topicName, List<(T @event, IntegrationEventExpand? eventMessageExpand)> events, CancellationToken stoppingToken = default)
    {
        // 参数验证
        if (events == null || events.Count == 0)
        {
            _logger?.LogWarning("批量发布接收到空事件列表, Topic: {Topic}", topicName);
            return;
        }
        try
        {
            var connection = await GetConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync();//创建通道
            await DeclareExchangeAndQueueAsync(channel, topicName, stoppingToken);//声明交换机、队列、绑定关系

            //// 注册确认事件处理器
            //channel.BasicAcksAsync += async (sender, args) =>
            //{
            //    // 成功确认
            //    Console.WriteLine($"Message {args.DeliveryTag} acknowledged");
            //    if (args.Multiple)
            //    {
            //        _logger.LogDebug($"All messages up to {args.DeliveryTag} acknowledged");
            //    }
            //};

            channel.BasicNacksAsync += async (sender, args) =>
            {
                // 否定确认（需要重试）
                _logger.LogError($"Message {args.DeliveryTag} nacked");
            };

            channel.BasicReturnAsync += async (sender, args) =>
            {
                // 消息无法路由（mandatory=true 时触发）
                _logger.LogError($"Message returned: {args.ReplyText}");
            };

            // 通道异常处理
            channel.CallbackExceptionAsync += async (sender, args) =>
            {
                _logger.LogError(args.Exception, "Channel callback exception");
            };

            channel.ChannelShutdownAsync += async (sender, args) =>
            {
                _logger.LogWarning($"Channel shutdown: {args.ReplyText}");
            };

            foreach (var (eventItem, _) in events)
            {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventItem));

                // 配置消息属性
                var properties = new BasicProperties
                {
                    Persistent = true,// 消息持久化：重启后消息不丢失
                    ContentType = "application/json",// 内容类型：JSON格式
                    DeliveryMode = DeliveryModes.Persistent, // 投递模式：持久化
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())// 事件戳
                };

                // 添加发布任务到列表（并行发布）
                await channel.BasicPublishAsync(
                    exchange: topicName,
                    routingKey: topicName + "_routingKey",
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: stoppingToken);
            }

            _logger?.LogInformation("批量发布成功, Topic: {Topic}, 消息数量: {Count}",
               topicName, events.Count);
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning("批量发布操作被用户取消, Topic: {Topic}", topicName);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "RabbitMQ批量发布失败, Topic: {Topic}, 消息数量: {Count}, Error: {ErrorMessage}",
                topicName, events.Count, ex.Message);
            throw new Exception($"RabbitMQ批量发布失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 获取或创建长连接（线程安全）
    /// </summary>
    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true }) return _connection;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true }) return _connection;

            _logger.LogInformation("正在创建 RabbitMQ 长连接...");
            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task DeclareExchangeAndQueueAsync(IChannel channel, string topicName, CancellationToken stoppingToken)
    {
        if (_declaredTopics.ContainsKey(topicName)) return;

        // 1. 声明死信交换机
        var deadLetterExchangeName = $"{topicName}_dead_letter_exchange";
        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        // 2. 声明死信队列
        var deadLetterQueueName = $"{topicName}_dead_letter_queue";
        await channel.QueueDeclareAsync(
            queue: deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        // 3. 绑定死信队列到死信交换机
        await channel.QueueBindAsync(
            queue: deadLetterQueueName,
            exchange: deadLetterExchangeName,
            routingKey: topicName + "_dead_letter_routingKey",
            arguments: null,
            cancellationToken: stoppingToken);

        // 7. 声明主交换机
        var exchangeArguments = new Dictionary<string, object>
        {
            { "x-delayed-type", "direct" } // 内部路由逻辑依然是 direct
        };
        await channel.ExchangeDeclareAsync(
            exchange: topicName,
            type: "x-delayed-message",
            durable: true,
            autoDelete: false,
            arguments: exchangeArguments,
            cancellationToken: stoppingToken);

        // 8. 声明主队列，修改死信配置指向重试交换机
        var mainQueueArguments = new Dictionary<string, object>
        {
            // 队列最大长度
            ["x-max-length"] = 10000,
            // 队列溢出行为
            ["x-overflow"] = "reject-publish"
        };

        var mainQueueName = topicName + "_queue";
        await channel.QueueDeclareAsync(
            queue: mainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArguments,
            cancellationToken: stoppingToken);

        // 9. 绑定主队列到主交换机
        await channel.QueueBindAsync(
            queue: mainQueueName,
            exchange: topicName,
            routingKey: topicName + "_routingKey",
            arguments: null,
            cancellationToken: stoppingToken);

        _logger?.LogDebug("交换机、队列声明完成，Topic: {TopicName}", topicName);
        _declaredTopics.TryAdd(topicName, true);
    }
}