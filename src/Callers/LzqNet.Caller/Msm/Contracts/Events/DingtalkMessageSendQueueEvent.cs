using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

namespace LzqNet.Caller.Msm.Contracts.Events;

public record DingtalkMessageSendQueueEvent : IntegrationEvent
{
    public override string Topic { get; set; } = "dingtalk.sendMessage";

    /// <summary>
    /// 推送消息Id
    /// </summary>
    public List<long> PushMessageRecordIds { get; set; }
}