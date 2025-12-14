using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Caller.Msm.Contracts.Menu.Queries;

public record MenuGetListQuery : Query<List<MenuViewDto>>
{
    public MenuSearchDto SearchDto { get; set; }
    public override List<MenuViewDto> Result { get; set; }
    public MenuGetListQuery(MenuSearchDto? searchDto)
    {
        SearchDto = searchDto ?? new();
    }
}
