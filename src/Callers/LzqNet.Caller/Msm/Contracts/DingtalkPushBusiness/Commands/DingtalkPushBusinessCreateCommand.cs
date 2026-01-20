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
        RuleFor(x => x.BusinessName)
           .NotEmpty().WithMessage("推送业务名不能为空")
           .MaximumLength(50).WithMessage("推送业务名长度不能超过50个字符");

        RuleFor(x => x.EnableStatus)
            .NotNull().WithMessage("启用状态不能为空")
            .IsInEnum().WithMessage("无效的启用状态值");

    }
}