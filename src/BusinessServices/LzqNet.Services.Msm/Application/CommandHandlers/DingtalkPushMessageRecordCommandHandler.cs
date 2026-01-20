using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Commands;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using LzqNet.Caller.Msm.Contracts.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DingtalkPushMessageRecordCommandHandler(
    IDingtalkPushMessageRecordRepository dingtalkPushMessageRecordRepository,
    IDingtalkPushBusinessRepository dingtalkPushBusinessRepository,
    IDingtalkPushConfigRepository dingtalkPushConfigRepository,
    IDingtalkPushRobotRepository dingtalkPushRobotRepository,
    IEventBus eventBus
    )
{
    private readonly IDingtalkPushMessageRecordRepository _dingtalkPushMessageRecordRepository = dingtalkPushMessageRecordRepository;
    private readonly IDingtalkPushBusinessRepository _dingtalkPushBusinessRepository = dingtalkPushBusinessRepository;
    private readonly IDingtalkPushConfigRepository _dingtalkPushConfigRepository = dingtalkPushConfigRepository;
    private readonly IDingtalkPushRobotRepository _dingtalkPushRobotRepository = dingtalkPushRobotRepository;
    private readonly IEventBus EventBus = eventBus;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushMessageRecordCreateCommand command)
    {
        var entity = command.Map<DingtalkPushMessageRecordEntity>();
        await _dingtalkPushMessageRecordRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushMessageRecordUpdateCommand command)
    {
        var entity = command.Map<DingtalkPushMessageRecordEntity>();
        await _dingtalkPushMessageRecordRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushMessageRecordDeleteCommand command)
    {
        await _dingtalkPushMessageRecordRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }

    [EventHandler]
    public async Task SendHandleAsync(DingtalkMessageSendCommand command)
    {
        var pushConfig = await _dingtalkPushConfigRepository.GetFirstAsync(a => a.PushConfigName.Equals(command.PushConfigName));
        if (pushConfig == null)
            throw new MasaValidatorException($"PushConfig with name {command.PushConfigName} not found.");
        var pushRobots = await _dingtalkPushRobotRepository.GetListAsync(a => pushConfig.PushRobotIds.Contains(a.Id));
        if (pushRobots == null || !pushRobots.Any())
            throw new MasaValidatorException($"PushRobot with id {pushConfig.PushRobotIds} not found.");
        var pushBusiness = await _dingtalkPushBusinessRepository.GetFirstAsync(a => a.Id.Equals(pushConfig.PushBusinessId));
        if (pushBusiness == null)
            throw new MasaValidatorException($"PushBusiness with id {pushConfig.PushBusinessId} not found.");

        var messageContent = pushConfig.PushTemplate;
        foreach (var item in command.TemplateParameters)
            messageContent.Replace("{{" + item.Key + "}}", item.Value);

        var pushMessageRecords = new List<DingtalkPushMessageRecordEntity>();
        foreach (var pushRobot in pushRobots)
        {
            var pushMessageRecord = new DingtalkPushMessageRecordEntity
            {
                PushBusinessName = pushBusiness.BusinessName,
                PushConfigName = pushConfig.PushConfigName,
                PushRobotName = pushRobot.Name,
                DingtalkGroupName = pushRobot.DingtalkGroupName,
                PushConfigType = pushConfig.PushConfigType,
                PushContent = messageContent,
                PushStatus = DingtalkPushStatusEnum.Pending,
                DingtalkUserIds = pushConfig.DingtalkUserIds,
                Webhook = pushRobot.Webhook,
                PushKeywords = pushRobot.PushKeywords,
                Sign = pushRobot.Sign,
                PushIpSegments = pushRobot.PushIpSegments
            };
            pushMessageRecords.Add(pushMessageRecord);
        }
        
        await _dingtalkPushMessageRecordRepository.InsertRangeAsync(pushMessageRecords);

        await EventBus.PublishAsync(new DingtalkMessageSendQueueEvent { PushMessageRecordIds = pushMessageRecords.Select(a=>a.Id).ToList() });

        var updateRecords = await _dingtalkPushMessageRecordRepository.GetListAsync(x => pushMessageRecords.Select(a => a.Id).Contains(x.Id));
        foreach (var item in updateRecords)
            item.PushStatus = DingtalkPushStatusEnum.Published;
        await _dingtalkPushMessageRecordRepository.UpdateRangeAsync(updateRecords);
    }

    [EventHandler]
    public async Task UpdateStatusHandleAsync(DingtalkPushMessageRecordUpdateStatusCommand command)
    {
        var entity = await _dingtalkPushMessageRecordRepository.GetFirstAsync(a=>a.Id.Equals(command.Id));
        if (entity == null)
            throw new MasaValidatorException($"DingtalkPushMessageRecord with id {command.Id} not found.");
        entity.PushStatus = command.PushStatus;
        await _dingtalkPushMessageRecordRepository.UpdateAsync(entity);
    }
}