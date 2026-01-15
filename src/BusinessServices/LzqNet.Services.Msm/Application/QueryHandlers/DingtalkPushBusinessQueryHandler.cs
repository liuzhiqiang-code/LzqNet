using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness;
using LzqNet.Caller.Msm.Contracts.DingtalkPushBusiness.Queries;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

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
        var searchDto = query.SearchDto;
        var paginatedOptions = new PaginatedOptions
        {
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
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