using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Enums;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_dingtalk_push_config")]
public class DingtalkPushConfigEntity : BaseFullEntity
{
    /// <summary>
    /// 关联业务Id
    /// </summary>
    [SugarColumn(ColumnName = "push_business_id")]
    public long PushBusinessId { get; set; }

    /// <summary>
    /// 推送机器人Id
    /// </summary>
    [SugarColumn(ColumnName = "push_robot_ids",IsJson = true)]
    public List<long>? PushRobotIds { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [SugarColumn(ColumnName = "push_config_name")]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [SugarColumn(ColumnName = "push_config_type")]
    public PushConfigTypeEnum PushConfigType { get; set; }

    /// <summary>
    /// 推送模板
    /// </summary>
    [SugarColumn(ColumnName = "push_template")]
    public string PushTemplate { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [SugarColumn(ColumnName = "enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_user_ids", IsJson = true)]
    public List<string> DingtalkUserIds { get; set; }

}