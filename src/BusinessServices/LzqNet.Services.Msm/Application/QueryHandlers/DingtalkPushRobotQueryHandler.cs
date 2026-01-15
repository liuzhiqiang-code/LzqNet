using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.Caller.Msm.Contracts.DingtalkPushRobot;
using LzqNet.Caller.Msm.Contracts.DingtalkPushRobot.Queries;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class DingtalkPushRobotQueryHandler(IDingtalkPushRobotRepository dingtalkPushRobotRepository)
{
    private readonly IDingtalkPushRobotRepository _dingtalkPushRobotRepository = dingtalkPushRobotRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DingtalkPushRobotListQuery query)
    {
        var list = (await _dingtalkPushRobotRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<DingtalkPushRobotViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(DingtalkPushRobotPageQuery query)
    {
        var searchDto = query.SearchDto;
        var paginatedOptions = new PaginatedOptions
        {
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };
        var pageList = await _dingtalkPushRobotRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<DingtalkPushRobotViewDto>>();
        query.Result = new PaginatedListBase<DingtalkPushRobotViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}