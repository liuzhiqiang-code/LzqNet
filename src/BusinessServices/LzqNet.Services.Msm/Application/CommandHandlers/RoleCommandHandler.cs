using LzqNet.Caller.Auth;
using LzqNet.Caller.Auth.Contracts;
using LzqNet.Caller.Msm.Contracts.Role.Commands;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class RoleCommandHandler(IRoleRepository roleRepository, AuthCaller authCaller)
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly AuthCaller _authCaller = authCaller;

    [EventHandler]
    public async Task CreateHandleAsync(RoleCreateCommand command)
    {
        var entity = command.Map<RoleEntity>();
        await _roleRepository.AddAsync(entity);

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
        await _roleRepository.UpdateAsync(entity);

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
        await _roleRepository.RemoveAsync(a => command.Ids.Contains(a.Id));

        var deleteRoleModels = list.Select(a => new RoleModel
        {
            Name = a.Name
        }).ToList();
        await _authCaller.DeleteRole(deleteRoleModels);
    }
}
