using Masa.Contrib.Dispatcher.Events;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;
using LzqNet.Caller.Msm.Contracts.Dept.Commands;

namespace LzqNet.Services.Msm.Application.CommandHandlers;

public class DeptCommandHandler(IDeptRepository deptRepository)
{
    private readonly IDeptRepository _deptRepository = deptRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DeptCreateCommand command)
    {
        var deptEntity = command.Map<DeptEntity>();
        await _deptRepository.AddAsync(deptEntity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DeptUpdateCommand command)
    {
        var deptEntity = command.Map<DeptEntity>();
        await _deptRepository.UpdateAsync(deptEntity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DeptDeleteCommand command)
    {
        await _deptRepository.RemoveAsync(a => command.Ids.Contains(a.Id));
    }
}
