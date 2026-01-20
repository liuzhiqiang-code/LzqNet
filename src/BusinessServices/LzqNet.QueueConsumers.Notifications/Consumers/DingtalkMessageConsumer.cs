using LzqNet.Caller.Msm.Contracts.Events;
using LzqNet.QueueConsumers.Notifications.CommandHandlers;
using Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq;
using Microsoft.Extensions.Options;

namespace LzqNet.QueueConsumers.Notifications.Consumers;

/// <summary>
/// 钉钉消息消费者
/// </summary>
public class DingtalkMessageConsumer : MessageConsumerBase<DingtalkMessageSendQueueEvent>
{
    public readonly DingtalkMessageService _service;
    public DingtalkMessageConsumer(
        DingtalkMessageService service,
        IOptions<RabbitMqOptions> options,
        ILogger<DingtalkMessageConsumer> logger)
        : base(options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Topic名称 - 必须与发布者一致
    /// </summary>
    protected override string TopicName => "dingtalk.sendMessage";


    /// <summary>
    /// 处理钉钉消息的业务逻辑
    /// </summary>
    protected override async Task<bool> ProcessMessageAsync(
        DingtalkMessageSendQueueEvent @event,
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

/// <summary>
/// 钉钉消息模型
/// </summary>
public class DingtalkMessage
{
    public string ToUser { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = "text";
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}