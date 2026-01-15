using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Commands;

public record DingtalkPushConfigCreateCommand : Command
{
    /// <summary>
    /// 关联业务Id
    /// </summary>
    public long PushBusinessId { get; set; }

    /// <summary>
    /// 推送机器人Id
    /// </summary>
    public long? PushRobotId { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    public int PushConfigType { get; set; }

    /// <summary>
    /// 推送模板
    /// </summary>
    public string PushTemplate { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum EnableStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    public string DingtalkUserIds { get; set; }

}
public class DingtalkPushConfigCreateCommandValidator : MasaAbstractValidator<DingtalkPushConfigCreateCommand>
{
    public DingtalkPushConfigCreateCommandValidator()
    {
    }
}