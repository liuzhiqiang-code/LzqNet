using LzqNet.Common.Contracts;
using LzqNet.System.Contracts.SysConfig;
using LzqNet.System.Contracts.SysConfig.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.System.Application.QueryHandlers;

public class SysConfigQueryHandler(ISysConfigRepository orderRepository)
{
    private readonly ISysConfigRepository _orderRepository = orderRepository;

    [EventHandler]
    public async Task GetListHandleAsync(SysConfigListQuery query)
    {
        query.Result = (await _orderRepository.GetListAsync()).ToList().Map<List<SysConfigViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(SysConfigPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _orderRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<SysConfigViewDto>>();
        query.Result = new PageList<SysConfigViewDto>(result, total);
    }
}
