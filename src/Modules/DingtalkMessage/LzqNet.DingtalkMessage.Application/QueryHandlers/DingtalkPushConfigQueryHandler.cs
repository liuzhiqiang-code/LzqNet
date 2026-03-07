using LzqNet.Common.Contracts;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Queries;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.DingtalkMessage.Application.QueryHandlers;

public class DingtalkPushConfigQueryHandler(IDingtalkPushConfigRepository dingtalkPushConfigRepository)
{
    private readonly IDingtalkPushConfigRepository _dingtalkPushConfigRepository = dingtalkPushConfigRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DingtalkPushConfigListQuery query)
    {
        var list = (await _dingtalkPushConfigRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<DingtalkPushConfigViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(DingtalkPushConfigPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _dingtalkPushConfigRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<DingtalkPushConfigViewDto>>();
        query.Result = new PageList<DingtalkPushConfigViewDto>(result, total);
    }
}