using LzqNet.Extensions.RabbitMq.Consumer;
using LzqNet.Extensions.RabbitMq.Publisher;
using LzqNet.Test.Consumer.Services;
using LzqNet.Test.Contracts.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LzqNet.Test.Consumer;

/// <summary>
/// 钉钉消息消费者
/// </summary>
public class TestConsumer : MessageConsumerBase<TestEvent>
{
    public readonly TestService _service;
    public TestConsumer(
        TestService service,
        IOptions<RabbitMqOptions> options,
        ILogger<TestConsumer> logger)
        : base(options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Topic名称 - 必须与发布者一致
    /// </summary>
    protected override string TopicName => "test.sendMessage";


    /// <summary>
    /// 处理钉钉消息的业务逻辑
    /// </summary>
    protected override async Task<bool> ProcessMessageAsync(
        TestEvent @event,
        string messageId,
        int retryCount,
        CancellationToken stoppingToken)
    {
        try
        {
            await _service.ProcessHandleAsync(@event);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理钉钉消息异常，消息ID: {MessageId}", messageId);
            return false;
        }
    }

    /// <summary>
    /// 自定义死信消息处理
    /// </summary>
    protected override async Task ProcessDeadLetterMessageAsync(
        string messageJson,
        string messageId,
        CancellationToken stoppingToken)
    {
        await base.ProcessDeadLetterMessageAsync(messageJson, messageId, stoppingToken);

        // 可以添加额外的死信处理逻辑，比如：
        // 1. 记录到数据库
        // 2. 发送告警通知
        // 3. 尝试其他发送渠道
    }
}