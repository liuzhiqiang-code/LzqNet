using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.System.Contracts.Role.Queries;

public record RoleListQuery : Query<List<RoleViewDto>>
{
    public long? Id { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }

    public string? Remark { get; set; }

    public override List<RoleViewDto> Result { get; set; }
    public RoleListQuery()
    {
    }
}