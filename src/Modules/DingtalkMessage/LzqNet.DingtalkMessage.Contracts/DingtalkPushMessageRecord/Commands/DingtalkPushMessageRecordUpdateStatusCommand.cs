using FluentValidation;
using FluentValidation.Validators;
using LzqNet.DingtalkMessage.Domain.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Commands;

public record DingtalkPushMessageRecordUpdateStatusCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 推送状态
    /// </summary>
    public DingtalkPushStatusEnum PushStatus { get; set; }
}
public class DingtalkPushMessageRecordUpdateStatusCommandValidator : MasaAbstractValidator<DingtalkPushMessageRecordUpdateStatusCommand>
{
    public DingtalkPushMessageRecordUpdateStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}