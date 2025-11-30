using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Role.Queries;

public record RoleGetListQuery : Query<List<RoleViewDto>>
{
    public RoleSearchDto SearchDto { get; set; }
    public override List<RoleViewDto> Result { get; set; }
    public RoleGetListQuery(RoleSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}
