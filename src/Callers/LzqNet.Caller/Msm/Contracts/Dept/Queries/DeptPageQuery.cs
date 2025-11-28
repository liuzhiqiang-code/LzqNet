using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Dept.Queries;

public record DeptPageQuery : Query<PaginatedListBase<DeptViewDto>>
{
    public DeptSearchDto SearchDto { get; set; }
    public override PaginatedListBase<DeptViewDto> Result { get; set; }
    public DeptPageQuery(DeptSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
