using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Queries;

public record DingtalkPushConfigListQuery : Query<List<DingtalkPushConfigViewDto>>
{
    public DingtalkPushConfigSearchDto SearchDto { get; set; }
    public override List<DingtalkPushConfigViewDto> Result { get; set; }
    public DingtalkPushConfigListQuery(DingtalkPushConfigSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}