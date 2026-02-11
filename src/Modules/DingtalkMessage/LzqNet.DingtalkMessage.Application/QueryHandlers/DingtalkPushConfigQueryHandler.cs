using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Queries;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig;

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
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
        };
        var pageList = await _dingtalkPushConfigRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<DingtalkPushConfigViewDto>>();
        query.Result = new PaginatedListBase<DingtalkPushConfigViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}