using LzqNet.AI.Contracts.AIChats;
using LzqNet.AI.Contracts.AIChats.Queries;
using LzqNet.AI.Domain.IRepositories;
using LzqNet.Common.Contracts;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.AI.Application.QueryHandlers;

public class AIChatsQueryHandler(IAIChatsRepository aiChatsRepository)
{
    private readonly IAIChatsRepository _aiChatsRepository = aiChatsRepository;

    [EventHandler]
    public async Task GetListHandleAsync(AIChatsListQuery query)
    {
        var list = (await _aiChatsRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<AIChatsViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(AIChatsPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _aiChatsRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<AIChatsViewDto>>();
        query.Result = new PageList<AIChatsViewDto>(result, total);
    }
}