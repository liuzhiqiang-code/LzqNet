using LzqNet.Caller.Auth;
using LzqNet.Caller.Auth.Contracts;
using LzqNet.Caller.Msm.Contracts.Role.Commands;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class RoleCommandHandler(IRoleRepository roleRepository,IRoleAuthRepository roleAuthRepository, AuthCaller authCaller)
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRoleAuthRepository _roleAuthRepository = roleAuthRepository;
    private readonly AuthCaller _authCaller = authCaller;

    [EventHandler]
    public async Task CreateHandleAsync(RoleCreateCommand command)
    {
        var entity = command.Map<RoleEntity>();
        entity = await _roleRepository.AddAsync(entity);
        var rolePermissions = command.Permissions.Select(permissionId => new RoleAuthEntity
        {
            RoleId = entity.Id,
            MenuId = permissionId
        }).ToList();

        if (rolePermissions.Count > 0)
            await _roleAuthRepository.AddRangeAsync(rolePermissions);

        await _authCaller.CreateRole(new RoleModel { 
            Name = entity.Name
        });
    }

    [EventHandler]
    public async Task UpdateHandleAsync(RoleUpdateCommand command)
    {
        var entity = await _roleRepository.FindAsync(command.Id);
        if (entity == null)
            throw new MasaValidatorException("Role not found");
        command.Map(entity);
        await _roleRepository.UpdateAsync(entity);
        await UpdateRolePermissionsAsync(command.Id, command.Permissions);

        var roleUpdateModel = new RoleUpdateModel
        {
            Name = entity.Name,
            NewRoleName = command.Name
        };
        await _authCaller.UpdateRole(roleUpdateModel);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(RoleDeleteCommand command)
    {
        var list = await _roleRepository.GetListAsync(a => command.Ids.Contains(a.Id));
        await DeleteRolePermissionsAsync(command.Ids);
        await _roleRepository.RemoveAsync(a => command.Ids.Contains(a.Id));

        var deleteRoleModels = list.Select(a => new RoleModel
        {
            Name = a.Name
        }).ToList();
        await _authCaller.DeleteRole(deleteRoleModels);
    }

    /// <summary>
    /// 更新角色权限
    /// </summary>
    private async Task UpdateRolePermissionsAsync(long roleId, List<long> newPermissionIds)
    {
        if (newPermissionIds == null)
            return;

        // 1. 获取现有的权限
        var existingPermissions = await _roleAuthRepository
            .GetListAsync(a => a.RoleId == roleId);

        // 2. 找出需要删除的权限（存在但现在不需要了）
        var permissionsToDelete = existingPermissions
            .Where(ep => !newPermissionIds.Contains(ep.MenuId))
            .ToList();

        if (permissionsToDelete.Any())
        {
            await _roleAuthRepository.RemoveAsync(a =>
                a.RoleId == roleId &&
                permissionsToDelete.Select(p => p.MenuId).Contains(a.MenuId));
        }

        // 3. 找出需要新增的权限（现在需要但之前没有的）
        var existingPermissionIds = existingPermissions.Select(ep => ep.MenuId).ToList();
        var permissionsToAdd = newPermissionIds
            .Where(pid => !existingPermissionIds.Contains(pid))
            .Select(permissionId => new RoleAuthEntity
            {
                RoleId = roleId,
                MenuId = permissionId
            })
            .ToList();

        if (permissionsToAdd.Any())
        {
            await _roleAuthRepository.AddRangeAsync(permissionsToAdd);
        }
    }

    /// <summary>
    /// 批量删除角色权限关联
    /// </summary>
    private async Task DeleteRolePermissionsAsync(List<long> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
            return;

        await _roleAuthRepository.RemoveAsync(a => roleIds.Contains(a.RoleId));
    }
}
