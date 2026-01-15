using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Commands;

public record DingtalkPushBusinessUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 推送业务名
    /// </summary>
    public string BusinessName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum EnableStatus { get; set; }

}
public class DingtalkPushBusinessUpdateCommandValidator : MasaAbstractValidator<DingtalkPushBusinessUpdateCommand>
{
    public DingtalkPushBusinessUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}