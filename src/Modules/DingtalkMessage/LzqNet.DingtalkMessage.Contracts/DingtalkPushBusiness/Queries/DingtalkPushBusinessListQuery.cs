using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;

public record DingtalkPushBusinessListQuery : Query<List<DingtalkPushBusinessViewDto>>
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

    public override List<DingtalkPushBusinessViewDto> Result { get; set; }
    public DingtalkPushBusinessListQuery()
    {
    }
}