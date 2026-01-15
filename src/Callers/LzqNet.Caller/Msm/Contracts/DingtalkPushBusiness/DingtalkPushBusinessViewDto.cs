
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness;

public class DingtalkPushBusinessViewDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 推送业务名
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public EnableStatusEnum? EnableStatus { get; set; }

}