using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Modeling.Queries;

public record ModelingPageQuery : Query<PaginatedListBase<ModelingViewDto>>
{
    public ModelingPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<ModelingViewDto> Result { get; set; }
    public ModelingPageQuery(ModelingPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}