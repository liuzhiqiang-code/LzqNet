using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Dept.Queries;

public record DeptGetTreeListQuery : Query<List<DeptViewDto>>
{
    public DeptSearchDto SearchDto { get; set; }
    public override List<DeptViewDto> Result { get; set; }
    public DeptGetTreeListQuery(DeptSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
