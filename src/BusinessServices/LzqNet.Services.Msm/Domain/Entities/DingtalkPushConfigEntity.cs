using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_dingtalk_push_config")]
public class DingtalkPushConfigEntity : BaseFullEntity
{
    /// <summary>
    /// 关联业务Id
    /// </summary>
    [Column("push_business_id")]
    public long PushBusinessId { get; set; }

    /// <summary>
    /// 推送机器人Id
    /// </summary>
    [Column("push_robot_id")]
    public long? PushRobotId { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    [Column("push_config_name")]
    public string PushConfigName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    [Column("push_config_type")]
    public int PushConfigType { get; set; }

    /// <summary>
    /// 推送模板
    /// </summary>
    [Column("push_template")]
    public string PushTemplate { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [Column("enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    [Column("dingtalk_user_ids")]
    public string DingtalkUserIds { get; set; }

}