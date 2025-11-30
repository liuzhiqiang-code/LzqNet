using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Role.Queries;

public record RolePageQuery : Query<PaginatedListBase<RoleViewDto>>
{
    public RolePageSearchDto SearchDto { get; set; }
    public override PaginatedListBase<RoleViewDto> Result { get; set; }
    public RolePageQuery(RolePageSearchDto searchDto)
    {
        SearchDto = searchDto;
    }
}
