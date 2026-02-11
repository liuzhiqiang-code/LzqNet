using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot.Queries;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot;

namespace LzqNet.DingtalkMessage.Application.QueryHandlers;

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
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
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