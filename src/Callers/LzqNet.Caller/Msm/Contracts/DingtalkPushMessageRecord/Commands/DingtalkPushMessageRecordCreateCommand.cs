using FluentValidation;
using FluentValidation.Validators;
using LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Enums;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Commands;

public record DingtalkPushMessageRecordCreateCommand : Command
{
    /// <summary>
    /// 关联业务名
    /// </summary>
    public string PushBusinessName { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送机器人名
    /// </summary>
    public string PushRobotName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    public PushConfigTypeEnum PushConfigType { get; set; }

    /// <summary>
    /// 推送内容
    /// </summary>
    public string PushContent { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public DingtalkPushStatusEnum PushStatus { get; set; } = DingtalkPushStatusEnum.Pending;

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    public List<string> DingtalkUserIds { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    public List<string> PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    public List<string> PushIpSegments { get; set; }

}
public class DingtalkPushMessageRecordCreateCommandValidator : MasaAbstractValidator<DingtalkPushMessageRecordCreateCommand>
{
    public DingtalkPushMessageRecordCreateCommandValidator()
    {
    }
}