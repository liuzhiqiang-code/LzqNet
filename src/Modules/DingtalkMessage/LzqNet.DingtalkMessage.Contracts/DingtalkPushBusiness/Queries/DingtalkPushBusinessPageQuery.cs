using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;

public record DingtalkPushBusinessPageQuery : Query<PaginatedListBase<DingtalkPushBusinessViewDto>>
{
    public DingtalkPushBusinessPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushBusinessViewDto> Result { get; set; }
    public DingtalkPushBusinessPageQuery(DingtalkPushBusinessPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}