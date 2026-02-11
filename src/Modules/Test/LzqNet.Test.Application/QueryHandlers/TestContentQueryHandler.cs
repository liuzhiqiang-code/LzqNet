using LzqNet.Test.Contracts.TestContent;
using LzqNet.Test.Contracts.TestContent.Queries;
using LzqNet.Test.Domain.IRepositories;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;

namespace LzqNet.Test.Application.QueryHandlers;

public class TestContentQueryHandler(ITestContentRepository testContentRepository)
{
    private readonly ITestContentRepository _testContentRepository = testContentRepository;

    [EventHandler]
    public async Task GetListHandleAsync(TestContentListQuery query)
    {
        var list = (await _testContentRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<TestContentViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(TestContentPageQuery query)
    {
        var paginatedOptions = new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize
        };
        var pageList = await _testContentRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<TestContentViewDto>>();
        query.Result = new PaginatedListBase<TestContentViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}