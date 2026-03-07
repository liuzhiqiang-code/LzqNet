using LzqNet.Common.Contracts;
using LzqNet.Test.Contracts.TestContentLog;
using LzqNet.Test.Contracts.TestContentLog.Queries;
using LzqNet.Test.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.Test.Application.QueryHandlers;

public class TestContentLogQueryHandler(ITestContentLogRepository testContentLogRepository)
{
    private readonly ITestContentLogRepository _testContentLogRepository = testContentLogRepository;

    [EventHandler]
    public async Task GetListHandleAsync(TestContentLogListQuery query)
    {
        var list = (await _testContentLogRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<TestContentLogViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(TestContentLogPageQuery query)
    {
        RefAsync<int> total = 0;
        var pageList = await _testContentLogRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<TestContentLogViewDto>>();
        query.Result = new PageList<TestContentLogViewDto>(result, total);
    }
}