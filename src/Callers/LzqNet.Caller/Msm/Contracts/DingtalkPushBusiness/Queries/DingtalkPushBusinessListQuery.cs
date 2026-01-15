using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Queries;

public record DingtalkPushBusinessListQuery : Query<List<DingtalkPushBusinessViewDto>>
{
    public DingtalkPushBusinessSearchDto SearchDto { get; set; }
    public override List<DingtalkPushBusinessViewDto> Result { get; set; }
    public DingtalkPushBusinessListQuery(DingtalkPushBusinessSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}