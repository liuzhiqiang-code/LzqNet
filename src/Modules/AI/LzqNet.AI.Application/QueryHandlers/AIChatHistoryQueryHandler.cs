using LzqNet.AI.Contracts.AIChatHistory;
using LzqNet.AI.Contracts.AIChatHistory.Queries;
using LzqNet.AI.Domain.IRepositories;
using LzqNet.Common.Contracts;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.AI.Application.QueryHandlers;

public class AIChatHistoryQueryHandler(IAIChatHistoryRepository aiChatHistoryRepository)
{
    private readonly IAIChatHistoryRepository _aiChatHistoryRepository = aiChatHistoryRepository;

    [EventHandler]
    public async Task GetListHandleAsync(AIChatHistoryListQuery query)
    {
        var list = (await _aiChatHistoryRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<AIChatHistoryViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(AIChatHistoryPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _aiChatHistoryRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<AIChatHistoryViewDto>>();
        query.Result = new PageList<AIChatHistoryViewDto>(result, total);
    }
}