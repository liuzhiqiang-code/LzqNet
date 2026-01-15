using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Modeling.Queries;

public record ModelingGetListQuery : Query<List<ModelingViewDto>>
{
    public ModelingSearchDto SearchDto { get; set; }
    public override List<ModelingViewDto> Result { get; set; }
    public ModelingGetListQuery(ModelingSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}