using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Queries;

public record DingtalkPushConfigPageQuery : Query<PaginatedListBase<DingtalkPushConfigViewDto>>
{
    public DingtalkPushConfigPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DingtalkPushConfigViewDto> Result { get; set; }
    public DingtalkPushConfigPageQuery(DingtalkPushConfigPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}