using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Dept.Queries;

public record DeptGetListQuery : Query<List<DeptViewDto>>
{
    public DeptSearchDto SearchDto { get; set; }
    public override List<DeptViewDto> Result { get; set; }
    public DeptGetListQuery(DeptSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}
