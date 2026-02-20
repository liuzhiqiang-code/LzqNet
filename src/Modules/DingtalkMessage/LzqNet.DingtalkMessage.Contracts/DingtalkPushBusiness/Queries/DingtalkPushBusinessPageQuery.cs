using LzqNet.Common.Contracts;
using Masa.Utils.Models;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;

public record DingtalkPushBusinessPageQuery : PageQuery<DingtalkPushBusinessViewDto>
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
    public int? EnableStatus { get; set; }

    public override PaginatedListBase<DingtalkPushBusinessViewDto> Result { get; set; }
    public DingtalkPushBusinessPageQuery()
    {
    }
}