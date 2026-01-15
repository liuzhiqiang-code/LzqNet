using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Commands;

public record DingtalkPushBusinessCreateCommand : Command
{
    /// <summary>
    /// 推送业务名
    /// </summary>
    public string BusinessName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum EnableStatus { get; set; }

}
public class DingtalkPushBusinessCreateCommandValidator : MasaAbstractValidator<DingtalkPushBusinessCreateCommand>
{
    public DingtalkPushBusinessCreateCommandValidator()
    {
    }
}