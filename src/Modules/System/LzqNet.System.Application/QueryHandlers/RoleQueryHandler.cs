using LzqNet.Common.Contracts;
using LzqNet.System.Contracts.Role;
using LzqNet.System.Contracts.Role.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.System.Application.QueryHandlers;

public class RoleQueryHandler(IRoleRepository roleRepository, IRoleAuthRepository roleAuthRepository)
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRoleAuthRepository _roleAuthRepository = roleAuthRepository;

    [EventHandler]
    public async Task GetListHandleAsync(RoleListQuery query)
    {
        var list = (await _roleRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<RoleViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(RolePageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _roleRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var roleIds = pageList.Select(r => r.Id).ToList();
        var rolePermissionsDict = await GetRolePermissionsDictionaryAsync(roleIds);
        var result = pageList.Select(role =>
        {
            var roleDto = role.Map<RoleViewDto>();
            if (rolePermissionsDict.TryGetValue(role.Id, out var permissions))
                roleDto.Permissions = permissions;
            else
                roleDto.Permissions = [];
            return roleDto;
        }).ToList();

        query.Result = new PageList<RoleViewDto>(result, total);
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
