using LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Commands;
using LzqNet.DingtalkMessage.Domain.Entities;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.DingtalkMessage.Application.CommandHandlers;

public class DingtalkPushBusinessCommandHandler(IDingtalkPushBusinessRepository dingtalkPushBusinessRepository)
{
    private readonly IDingtalkPushBusinessRepository _dingtalkPushBusinessRepository = dingtalkPushBusinessRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DingtalkPushBusinessCreateCommand command)
    {
        var isExists = await _dingtalkPushBusinessRepository.IsAnyAsync(a => a.BusinessName == command.BusinessName);
        if (isExists)
            throw new MasaValidatorException("推送业务名已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushBusinessEntity>();
        await _dingtalkPushBusinessRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DingtalkPushBusinessUpdateCommand command)
    {
        var isExists = await _dingtalkPushBusinessRepository.IsAnyAsync(a => a.BusinessName == command.BusinessName && a.Id != command.Id);
        if (isExists)
            throw new MasaValidatorException("推送业务名已存在，请使用其他名称");

        var entity = command.Map<DingtalkPushBusinessEntity>();
        await _dingtalkPushBusinessRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DingtalkPushBusinessDeleteCommand command)
    {
        await _dingtalkPushBusinessRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}