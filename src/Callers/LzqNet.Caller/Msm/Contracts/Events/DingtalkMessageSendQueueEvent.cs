using FluentValidation;
using FluentValidation.Validators;
using LzqNet.Caller.Msm.Contracts.Test.Commands;
using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Events;

public record DingtalkMessageSendQueueEvent : IntegrationEvent
{
    public override string Topic { get; set; } = "dingtalk.sendMessage";

    /// <summary>
    /// 推送消息Id
    /// </summary>
    public long PushMessageRecordId { get; set; }
}