using LzqNet.Common.Contracts;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.DingtalkMessage.Application.QueryHandlers;

public class DingtalkPushBusinessQueryHandler(IDingtalkPushBusinessRepository dingtalkPushBusinessRepository)
{
    private readonly IDingtalkPushBusinessRepository _dingtalkPushBusinessRepository = dingtalkPushBusinessRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DingtalkPushBusinessListQuery query)
    {
        var list = (await _dingtalkPushBusinessRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<DingtalkPushBusinessViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(DingtalkPushBusinessPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _dingtalkPushBusinessRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<DingtalkPushBusinessViewDto>>();
        query.Result = new PageList<DingtalkPushBusinessViewDto>(result, total);
    }
}