using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.QueueConsumers.Notifications.Entities;

[SugarTable("msm_dingtalk_push_message_record")]
public class DingtalkPushMessageRecordEntity
{
    public long Id { get; private set; }

    /// <summary>
    /// 关联业务名
    /// </summary>
    [SugarColumn(ColumnName = "push_business_name")]
    public string PushBusinessName { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [SugarColumn(ColumnName = "push_config_name")]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送机器人名
    /// </summary>
    [SugarColumn(ColumnName = "push_robot_name")]
    public string PushRobotName { get; set; }

    /// <summary>
    /// 推送群组名
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_group_name")]
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [SugarColumn(ColumnName = "push_config_type")]
    public int PushConfigType { get; set; }

    /// <summary>
    /// 推送内容
    /// </summary>
    [SugarColumn(ColumnName = "push_content")]
    public string PushContent { get; set; }

    /// <summary>
    /// 推送状态
    /// </summary>
    [SugarColumn(ColumnName = "push_status")]
    public DingtalkPushStatusEnum PushStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_user_ids")]
    public string DingtalkUserIds { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    [SugarColumn(ColumnName = "webhook")]
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    [SugarColumn(ColumnName = "push_keywords")]
    public string PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    [SugarColumn(ColumnName = "sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    [SugarColumn(ColumnName = "push_ip_segments")]
    public string PushIpSegments { get; set; }

    [SugarColumn(ColumnName = "creator")]
    public long Creator { get; set; }

    [SugarColumn(ColumnName = "creation_time")]
    public DateTime CreationTime { get; set; }

    [SugarColumn(ColumnName = "modifier")]
    public long Modifier { get; set; }

    [SugarColumn(ColumnName = "modification_time")]
    public DateTime ModificationTime { get; set; }

    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; }
}
