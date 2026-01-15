using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Commands;

public record DingtalkMessageSendCommand : Command
{
    /// <summary>
    /// 推送配置名
    /// </summary>
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送配置参数键值对（用于模板参数替换）
    /// </summary>
    public Dictionary<string, string> TemplateParameters { get; set; } = new();

}
public class DingtalkMessageSendCommandValidator : MasaAbstractValidator<DingtalkMessageSendCommand>
{
    public DingtalkMessageSendCommandValidator()
    {
        RuleFor(x => x.PushConfigName)
            .NotNull().WithMessage("推送配置名不能为null")
            .NotEmpty().WithMessage("推送配置名不能为空");
    }
}