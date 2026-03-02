using LzqNet.Common.Contracts;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Enums;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Enums;
using SqlSugar;

namespace LzqNet.DingtalkMessage.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_dingtalk_push_message_record")]
public class DingtalkPushMessageRecordEntity : BaseFullEntity
{
    /// <summary>
    /// 关联业务名
    /// </summary>
    [SugarColumn(ColumnName = "push_business_name", Length = 100)]
    public string PushBusinessName { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [SugarColumn(ColumnName = "push_config_name", Length = 100)]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送机器人名
    /// </summary>
    [SugarColumn(ColumnName = "push_robot_name", Length = 100)]
    public string PushRobotName { get; set; }

    /// <summary>
    /// 推送机器人名
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_group_name", Length = 100)]
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [SugarColumn(ColumnName = "push_config_type")]
    public PushConfigTypeEnum PushConfigType { get; set; }

    /// <summary>
    /// 推送内容
    /// </summary>
    [SugarColumn(ColumnName = "push_content", Length = 2000)]
    public string PushContent { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [SugarColumn(ColumnName = "push_status")]
    public DingtalkPushStatusEnum PushStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_user_ids", IsJson = true)]
    public List<string> DingtalkUserIds { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    [SugarColumn(ColumnName = "webhook", Length = 500)]
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    [SugarColumn(ColumnName = "push_keywords", IsJson = true)]
    public List<string> PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    [SugarColumn(ColumnName = "sign", Length = 500)]
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    [SugarColumn(ColumnName = "push_ip_segments", IsJson = true)]
    public List<string> PushIpSegments { get; set; }

    /// <summary>
    /// 推送返回结果
    /// </summary>
    [SugarColumn(ColumnName = "push_return_message", Length = 1000)]
    public string? PushReturnMessage { get; set; }
}