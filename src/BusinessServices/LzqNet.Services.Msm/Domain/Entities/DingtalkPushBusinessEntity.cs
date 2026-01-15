using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_dingtalk_push_business")]
public class DingtalkPushBusinessEntity : BaseFullEntity
{
    /// <summary>
    /// 推送业务名
    /// </summary>
    [Column("business_name")]
    public string BusinessName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [Column("enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

}