using LzqNet.Caller.Common.Contracts;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Modeling.Model.Queries;

public record ModelSelectListQuery : Query<List<SelectViewDto>>
{
    public ModelSearchDto SearchDto { get; set; }
    public override List<SelectViewDto> Result { get; set; }
    public ModelSelectListQuery(ModelSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
