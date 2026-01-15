using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Commands;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DingtalkPushBusinessCommandHandler(IDingtalkPushBusinessRepository dingtalkPushBusinessRepository)
{
    private readonly IDingtalkPushBusinessRepository _dingtalkPushBusinessRepository = dingtalkPushBusinessRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushBusinessCreateCommand command)
    {
        var entity = command.Map<DingtalkPushBusinessEntity>();
        await _dingtalkPushBusinessRepository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushBusinessUpdateCommand command)
    {
        var entity = command.Map<DingtalkPushBusinessEntity>();
        await _dingtalkPushBusinessRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushBusinessDeleteCommand command)
    {
        await _dingtalkPushBusinessRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}