using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Commands;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DingtalkPushConfigCommandHandler(IDingtalkPushConfigRepository dingtalkPushConfigRepository)
{
    private readonly IDingtalkPushConfigRepository _dingtalkPushConfigRepository = dingtalkPushConfigRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushConfigCreateCommand command)
    {
        var entity = command.Map<DingtalkPushConfigEntity>();
        await _dingtalkPushConfigRepository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushConfigUpdateCommand command)
    {
        var entity = command.Map<DingtalkPushConfigEntity>();
        await _dingtalkPushConfigRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushConfigDeleteCommand command)
    {
        await _dingtalkPushConfigRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}