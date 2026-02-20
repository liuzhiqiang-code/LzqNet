using LzqNet.Test.Contracts.TestContentLog;
using LzqNet.Test.Contracts.TestContentLog.Queries;
using LzqNet.Test.Domain.IRepositories;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;

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
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
        };
        var pageList = await _testContentLogRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<TestContentLogViewDto>>();
        query.Result = new PaginatedListBase<TestContentLogViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}