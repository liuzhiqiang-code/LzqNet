using LzqNet.Caller.Common.Contracts;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_dingtalk_push_business")]
public class DingtalkPushBusinessEntity : BaseFullEntity
{
    /// <summary>
    /// 推送业务名
    /// </summary>
    [SugarColumn(ColumnName = "business_name")]
    public string BusinessName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [SugarColumn(ColumnName = "enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

}