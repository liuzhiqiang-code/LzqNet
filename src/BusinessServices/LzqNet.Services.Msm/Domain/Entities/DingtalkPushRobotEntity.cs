using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_dingtalk_push_robot")]
public class DingtalkPushRobotEntity : BaseFullEntity
{
    /// <summary>
    /// 机器人名
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 所属钉钉群组
    /// </summary>
    [Column("dingtalk_group_name")]
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [Column("enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    [Column("webhook")]
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    [Column("push_keywords")]
    public string PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    [Column("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    [Column("push_ip_segments")]
    public string PushIpSegments { get; set; }

}