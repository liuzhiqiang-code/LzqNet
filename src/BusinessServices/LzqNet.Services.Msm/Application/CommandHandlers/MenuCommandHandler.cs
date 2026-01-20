using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.Menu.Commands;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class MenuCommandHandler(IMenuRepository MenuRepository)
{
    private readonly IMenuRepository _MenuRepository = MenuRepository;

    [EventHandler]
    public async Task CreateHandleAsync(MenuCreateCommand command)
    {
        var entity = command.Map<MenuEntity>();
        await _MenuRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(MenuUpdateCommand command)
    {
        var entity = command.Map<MenuEntity>();
        await _MenuRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(MenuDeleteCommand command)
    {
        await _MenuRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}
