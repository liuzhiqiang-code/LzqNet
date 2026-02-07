using Masa.Contrib.Dispatcher.Events;
using LzqNet.System.Contracts.SysConfig.Commands;
using LzqNet.System.Domain.Entities;
using LzqNet.System.Domain.IRepositories;

namespace LzqNet.System.Application.CommandHandlers;

public class SysConfigCommandHandler(ISysConfigRepository sysConfigRepository)
{
    private readonly ISysConfigRepository _sysConfigRepository = sysConfigRepository;

    [EventHandler]
    public async Task CreateHandleAsync(SysConfigCreateCommand command)
    {
        var sysConfigEntity = command.Map<SysConfigEntity>();
        await _sysConfigRepository.InsertAsync(sysConfigEntity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(SysConfigUpdateCommand command)
    {
        var sysConfigEntity = command.Map<SysConfigEntity>();
        await _sysConfigRepository.UpdateAsync(sysConfigEntity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(SysConfigDeleteCommand command)
    {
        await _sysConfigRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}
