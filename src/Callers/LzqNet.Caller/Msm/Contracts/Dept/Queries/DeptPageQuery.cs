using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Dept.Queries;

public record DeptPageQuery : Query<PaginatedListBase<DeptViewDto>>
{
    public DeptPageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DeptViewDto> Result { get; set; }
    public DeptPageQuery(DeptPageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
