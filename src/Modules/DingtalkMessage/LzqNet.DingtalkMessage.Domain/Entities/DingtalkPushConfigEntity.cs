using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Enums;
using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.DingtalkMessage.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_dingtalk_push_config")]
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
    [SugarColumn(ColumnName = "push_robot_ids", IsJson = true)]
    public List<long>? PushRobotIds { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [SugarColumn(ColumnName = "push_config_name", Length = 100)]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [SugarColumn(ColumnName = "push_config_type")]
    public PushConfigTypeEnum PushConfigType { get; set; }

    /// <summary>
    /// 推送模板
    /// </summary>
    [SugarColumn(ColumnName = "push_template", Length = 2000)]
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