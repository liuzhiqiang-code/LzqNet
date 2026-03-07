using LzqNet.Common.Contracts;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot.Queries;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

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
        RefAsync<int> total = 0;
        var pageList = await _dingtalkPushRobotRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<DingtalkPushRobotViewDto>>();
        query.Result = new PageList<DingtalkPushRobotViewDto>(result, total);
    }
}