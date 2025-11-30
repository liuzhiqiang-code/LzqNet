using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.Caller.Msm.Contracts.Role;
using LzqNet.Caller.Msm.Contracts.Role.Queries;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class RoleQueryHandler(IRoleRepository RoleRepository)
{
    private readonly IRoleRepository _RoleRepository = RoleRepository;

    [EventHandler]
    public async Task GetListHandleAsync(RoleGetListQuery query)
    {
        query.Result = (await _RoleRepository.GetListAsync())
            .ToList()
            .Map<List<RoleViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(RolePageQuery query)
    {
        PaginatedOptions paginatedOptions = new() {
            Page =  query.SearchDto.Page,
            PageSize = query.SearchDto.PageSize
        };
        var pageList = await _RoleRepository.GetPaginatedListAsync(paginatedOptions);
        query.Result = new PaginatedListBase<RoleViewDto>
        {
            Result = pageList.Result.Map<List<RoleViewDto>>(),
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}
