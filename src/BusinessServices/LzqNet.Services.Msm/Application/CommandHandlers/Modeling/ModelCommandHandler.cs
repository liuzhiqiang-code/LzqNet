//using LzqNet.Caller.Msm.Modeling.Model.Commands;
//using LzqNet.Services.Msm.Domain.Entities.Modeling;
//using Masa.BuildingBlocks.Ddd.Domain.Repositories;
//using Masa.Contrib.Dispatcher.Events;

//namespace LzqNet.Services.Msm.Application.CommandHandlers.Modeling;

//public class ModelCommandHandler(IRepository<ModelingEntity, long> repository)
//{
//    private readonly IRepository<ModelingEntity, long> _repository = repository;

//    [EventHandler]
//    public async Task CreateHandleAsync(ModelCreateCommand command)
//    {
//        var entity = command.Map<ModelingEntity>();
//        await _repository.AddAsync(entity);
//    }

//    [EventHandler]
//    public async Task UpdateHandleAsync(ModelUpdateCommand command)
//    {
//        var entity = command.Map<ModelingEntity>();
//        await _repository.UpdateAsync(entity);
//    }

//    [EventHandler]
//    public async Task DeleteHandleAsync(ModelDeleteCommand command)
//    {
//        await _repository.RemoveAsync(a => command.Ids.Contains(a.Id));
//    }
//}