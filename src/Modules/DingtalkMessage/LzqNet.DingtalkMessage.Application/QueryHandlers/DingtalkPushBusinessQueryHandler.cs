using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness;

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
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
        };
        var pageList = await _dingtalkPushBusinessRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<DingtalkPushBusinessViewDto>>();
        query.Result = new PaginatedListBase<DingtalkPushBusinessViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}