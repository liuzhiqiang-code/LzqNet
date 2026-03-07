using LzqNet.Common.Contracts;
using LzqNet.Test.Contracts.TestContent;
using LzqNet.Test.Contracts.TestContent.Queries;
using LzqNet.Test.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

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
        RefAsync<int> total = 0;
        var pageList = await _testContentRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<TestContentViewDto>>();
        query.Result = new PageList<TestContentViewDto>(result, total);
    }
}