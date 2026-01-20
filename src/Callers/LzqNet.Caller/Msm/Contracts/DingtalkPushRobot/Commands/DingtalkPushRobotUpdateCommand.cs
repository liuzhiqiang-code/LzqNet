using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushRobot.Commands;

public record DingtalkPushRobotUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 机器人名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 所属钉钉群组
    /// </summary>
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum? EnableStatus { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    public List<string> PushKeywords { get; set; } = new();

    /// <summary>
    /// 加签
    /// </summary>
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    public List<string> PushIpSegments { get; set; } = new();

}
public class DingtalkPushRobotUpdateCommandValidator : MasaAbstractValidator<DingtalkPushRobotUpdateCommand>
{
    public DingtalkPushRobotUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");

        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("机器人名不能为空")
           .MaximumLength(50).WithMessage("机器人名长度不能超过50个字符");

        RuleFor(x => x.DingtalkGroupName)
            .NotEmpty().WithMessage("所属群组不能为空")
            .MaximumLength(100).WithMessage("群组名称长度不能超过100个字符");

        RuleFor(x => x.EnableStatus)
            .NotNull().WithMessage("启用状态不能为空")
            .IsInEnum().WithMessage("无效的启用状态值");

        RuleFor(x => x.Webhook)
            .NotEmpty().WithMessage("推送地址不能为空")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("无效的Webhook地址格式");

        RuleFor(x => x.Sign)
            .NotEmpty().WithMessage("加签不能为空");
    }
}