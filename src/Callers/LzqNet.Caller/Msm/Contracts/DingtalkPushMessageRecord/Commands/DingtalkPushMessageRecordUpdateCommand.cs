using FluentValidation;
using FluentValidation.Validators;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Commands;

public record DingtalkPushMessageRecordUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

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
    public int PushConfigType { get; set; }

    /// <summary>
    /// 推送内容
    /// </summary>
    public string PushContent { get; set; }

    /// <summary>
    /// 推送状态
    /// </summary>
    public DingtalkPushStatusEnum PushStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    public string DingtalkUserIds { get; set; }

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
public class DingtalkPushMessageRecordUpdateCommandValidator : MasaAbstractValidator<DingtalkPushMessageRecordUpdateCommand>
{
    public DingtalkPushMessageRecordUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}