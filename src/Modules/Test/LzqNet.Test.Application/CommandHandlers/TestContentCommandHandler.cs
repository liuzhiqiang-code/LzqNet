using LzqNet.Test.Contracts.TestContent.Commands;
using LzqNet.Test.Domain.Entities;
using LzqNet.Test.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.Test.Application.CommandHandlers;

public class TestContentCommandHandler(ITestContentRepository testContentRepository)
{
    private readonly ITestContentRepository _testContentRepository = testContentRepository;

    [EventHandler]
    public async Task CreateHandleAsync(TestContentCreateCommand command)
    {
        var entity = command.Map<TestContentEntity>();
        await _testContentRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(TestContentUpdateCommand command)
    {
        var entity = command.Map<TestContentEntity>();
        await _testContentRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(TestContentDeleteCommand command)
    {
        await _testContentRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}