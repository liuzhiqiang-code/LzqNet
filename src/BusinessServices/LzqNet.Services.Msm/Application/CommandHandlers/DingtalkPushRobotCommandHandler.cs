using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.DingtalkPushRobot.Commands;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DingtalkPushRobotCommandHandler(IDingtalkPushRobotRepository dingtalkPushRobotRepository)
{
    private readonly IDingtalkPushRobotRepository _dingtalkPushRobotRepository = dingtalkPushRobotRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushRobotCreateCommand command)
    {
        var isExists = await _dingtalkPushRobotRepository.IsAnyAsync(a => a.Name == command.Name);
        if (isExists)
            throw new MasaValidatorException("名称已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushRobotEntity>();
        await _dingtalkPushRobotRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushRobotUpdateCommand command)
    {
        var isExists = await _dingtalkPushRobotRepository.IsAnyAsync(a => a.Name == command.Name && a.Id != command.Id);
        if (isExists)
            throw new MasaValidatorException("名称已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushRobotEntity>();
        await _dingtalkPushRobotRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushRobotDeleteCommand command)
    {
        await _dingtalkPushRobotRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}