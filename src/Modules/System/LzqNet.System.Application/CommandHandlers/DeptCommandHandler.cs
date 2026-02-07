using Masa.Contrib.Dispatcher.Events;
using LzqNet.System.Contracts.Dept.Commands;
using LzqNet.System.Domain.Entities;
using LzqNet.System.Domain.IRepositories;

namespace LzqNet.System.Application.CommandHandlers;

public class DeptCommandHandler(IDeptRepository deptRepository)
{
    private readonly IDeptRepository _deptRepository = deptRepository;

    [EventHandler]
    public async Task CreateHandleAsync(DeptCreateCommand command)
    {
        var entity = command.Map<DeptEntity>();
        await _deptRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(DeptUpdateCommand command)
    {
        var entity = command.Map<DeptEntity>();
        await _deptRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(DeptDeleteCommand command)
    {
        await _deptRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}
