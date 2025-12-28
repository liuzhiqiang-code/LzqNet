using LzqNet.Caller.Msm.Contracts.Role;
using LzqNet.Caller.Msm.Contracts.Role.Queries;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class RoleQueryHandler(IRoleRepository roleRepository, IRoleAuthRepository roleAuthRepository)
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRoleAuthRepository _roleAuthRepository = roleAuthRepository;

    [EventHandler]
    public async Task GetListHandleAsync(RoleGetListQuery query)
    {
        var list = (await _roleRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<RoleViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(RolePageQuery query)
    {
        var searchDto = query.SearchDto;
        var paginatedOptions = new PaginatedOptions
        {
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };
        var pageList = await _roleRepository.GetPaginatedListAsync(paginatedOptions);
        var roleIds = pageList.Result.Select(r => r.Id).ToList();
        var rolePermissionsDict = await GetRolePermissionsDictionaryAsync(roleIds);
        var result = pageList.Result.Select(role =>
        {
            var roleDto = role.Map<RoleViewDto>();
            if (rolePermissionsDict.TryGetValue(role.Id, out var permissions))
                roleDto.Permissions = permissions;
            else
                roleDto.Permissions = [];
            return roleDto;
        }).ToList();
        query.Result = new PaginatedListBase<RoleViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }

    private async Task<Dictionary<long, List<long>>> GetRolePermissionsDictionaryAsync(List<long> roleIds)
    {
        var rolePermissions = await _roleAuthRepository
            .GetListAsync(a => roleIds.Contains(a.RoleId));
        return rolePermissions
            .GroupBy(ra => ra.RoleId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(ra => ra.MenuId).ToList()
            );
    }
}
