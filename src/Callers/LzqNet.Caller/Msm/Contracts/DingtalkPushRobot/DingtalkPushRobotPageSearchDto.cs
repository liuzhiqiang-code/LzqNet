using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushRobot;

public class DingtalkPushRobotPageSearchDto : RequestPageBase
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 机器人名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 所属钉钉群组
    /// </summary>
    public string? DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum? EnableStatus { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    public string? Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    public string? PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    public string? Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    public string? PushIpSegments { get; set; }

}