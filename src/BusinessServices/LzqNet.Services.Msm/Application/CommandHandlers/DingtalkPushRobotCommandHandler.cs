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
        var entity = command.Map<DingtalkPushRobotEntity>();
        await _dingtalkPushRobotRepository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushRobotUpdateCommand command)
    {
        var entity = command.Map<DingtalkPushRobotEntity>();
        await _dingtalkPushRobotRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushRobotDeleteCommand command)
    {
        await _dingtalkPushRobotRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}