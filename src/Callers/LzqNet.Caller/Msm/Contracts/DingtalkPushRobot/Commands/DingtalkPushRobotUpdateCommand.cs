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
    public string PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    public string PushIpSegments { get; set; }

}
public class DingtalkPushRobotUpdateCommandValidator : MasaAbstractValidator<DingtalkPushRobotUpdateCommand>
{
    public DingtalkPushRobotUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}