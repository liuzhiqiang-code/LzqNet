using LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Commands;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DingtalkPushConfigCommandHandler(IDingtalkPushConfigRepository dingtalkPushConfigRepository)
{
    private readonly IDingtalkPushConfigRepository _dingtalkPushConfigRepository = dingtalkPushConfigRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushConfigCreateCommand command)
    {
        var isExists = await _dingtalkPushConfigRepository.IsAnyAsync(a => a.PushConfigName == command.PushConfigName 
            && a.PushBusinessId.Equals(command.PushBusinessId));
        if (isExists)
            throw new MasaValidatorException("推送配置名已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushConfigEntity>();
        await _dingtalkPushConfigRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushConfigUpdateCommand command)
    {
        var isExists = await _dingtalkPushConfigRepository.IsAnyAsync(a => a.PushConfigName == command.PushConfigName
            && a.PushBusinessId.Equals(command.PushBusinessId));
        if (isExists)
            throw new MasaValidatorException("推送配置名已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushConfigEntity>();
        await _dingtalkPushConfigRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushConfigDeleteCommand command)
    {
        await _dingtalkPushConfigRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}