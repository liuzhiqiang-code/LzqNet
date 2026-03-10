using LzqNet.AI.Contracts.AIChatHistory.Commands;
using LzqNet.AI.Domain.Entities;
using LzqNet.AI.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;

namespace LzqNet.AI.Application.CommandHandlers;

public class AiChatHistoryCommandHandler(IAIChatHistoryRepository aiChatHistoryRepository)
{
    private readonly IAIChatHistoryRepository _aiChatHistoryRepository = aiChatHistoryRepository;

    [EventHandler]
    public async Task CreateHandleAsync(AIChatHistoryCreateCommand command)
    {
        var entity = command.Map<AIChatHistoryEntity>();
        await _aiChatHistoryRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(AIChatHistoryUpdateCommand command)
    {
        var entity = command.Map<AIChatHistoryEntity>();
        await _aiChatHistoryRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(AIChatHistoryDeleteCommand command)
    {
        await _aiChatHistoryRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }
}