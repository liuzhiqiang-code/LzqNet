using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_dingtalk_push_message_record")]
public class DingtalkPushMessageRecordEntity : BaseFullEntity
{
    /// <summary>
    /// 关联业务名
    /// </summary>
    [Column("push_business_name")]
    public string PushBusinessName { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [Column("push_config_name")]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送机器人名
    /// </summary>
    [Column("push_robot_name")]
    public string PushRobotName { get; set; }

    /// <summary>
    /// 推送群组名
    /// </summary>
    [Column("dingtalk_group_name")]
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [Column("push_config_type")]
    public int PushConfigType { get; set; }

    /// <summary>
    /// 推送内容
    /// </summary>
    [Column("push_content")]
    public string PushContent { get; set; }

    /// <summary>
    /// 推送状态
    /// </summary>
    [Column("push_status")]
    public DingtalkPushStatusEnum PushStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    [Column("dingtalk_user_ids")]
    public string DingtalkUserIds { get; set; }

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