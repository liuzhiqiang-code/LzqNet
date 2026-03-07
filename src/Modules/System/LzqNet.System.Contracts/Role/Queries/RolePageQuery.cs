using LzqNet.Common.Contracts;

namespace LzqNet.System.Contracts.Role.Queries;

public record RolePageQuery : PageQuery<RoleViewDto>
{
    public long? Id { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }

    public string? Remark { get; set; }

    public override PageList<RoleViewDto> Result { get; set; }
    public RolePageQuery()
    {
    }
}