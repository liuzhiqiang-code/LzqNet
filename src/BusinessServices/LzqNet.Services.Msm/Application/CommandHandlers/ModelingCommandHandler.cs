using LzqNet.Caller.Msm.Contracts.Modeling.Commands;
using LzqNet.Services.Msm.Domain.Entities.Modeling;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class ModelingCommandHandler(IModelingRepository modelingRepository)
{
    private readonly IModelingRepository _modelingRepository = modelingRepository;

    [EventHandler]
    public async Task CreateHandleAsync(ModelingCreateCommand command)
    {
        var entity = command.Map<ModelingEntity>();
        await _modelingRepository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(ModelingUpdateCommand command)
    {
        var entity = command.Map<ModelingEntity>();
        await _modelingRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(ModelingDeleteCommand command)
    {
        await _modelingRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}