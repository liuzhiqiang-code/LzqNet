using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.System.Contracts.SysConfig.Queries;
using LzqNet.System.Contracts.SysConfig;
using LzqNet.System.Domain.IRepositories;

namespace LzqNet.System.Application.QueryHandlers;

public class SysConfigQueryHandler(ISysConfigRepository orderRepository)
{
    private readonly ISysConfigRepository _orderRepository = orderRepository;

    [EventHandler]
    public async Task GetListHandleAsync(SysConfigGetListQuery query)
    {
        query.Result = (await _orderRepository.GetListAsync()).ToList().Map<List<SysConfigViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(SysConfigPageQuery query)
    {
        PaginatedOptions paginatedOptions = new() {
            Page =  query.SearchDto.Page,
            PageSize = query.SearchDto.PageSize
        };
        var pageList = await _orderRepository.GetPaginatedListAsync(paginatedOptions);
        query.Result = new PaginatedListBase<SysConfigViewDto>
        {
            Result = pageList.Result.Map<List<SysConfigViewDto>>(),
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}
