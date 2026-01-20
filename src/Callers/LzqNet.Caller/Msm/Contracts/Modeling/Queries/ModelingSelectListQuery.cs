using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.Modeling;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Modeling.Queries;

public record ModelingSelectListQuery : Query<List<SelectViewDto>>
{
    public ModelingSearchDto SearchDto { get; set; }
    public override List<SelectViewDto> Result { get; set; }
    public ModelingSelectListQuery(ModelingSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}
