using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Queries;

public record DingtalkPushBusinessPageQuery : Query<PaginatedListBase<DingtalkPushBusinessViewDto>>
{
    public DingtalkPushBusinessPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushBusinessViewDto> Result { get; set; }
    public DingtalkPushBusinessPageQuery(DingtalkPushBusinessPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}