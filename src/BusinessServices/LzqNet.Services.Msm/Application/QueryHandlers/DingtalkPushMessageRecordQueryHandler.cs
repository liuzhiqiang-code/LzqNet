using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Models;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Queries;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Application.QueryHandlers;

public class DingtalkPushMessageRecordQueryHandler(IDingtalkPushMessageRecordRepository dingtalkPushMessageRecordRepository)
{
    private readonly IDingtalkPushMessageRecordRepository _dingtalkPushMessageRecordRepository = dingtalkPushMessageRecordRepository;

    [EventHandler]
    public async Task GetListHandleAsync(DingtalkPushMessageRecordListQuery query)
    {
        var list = (await _dingtalkPushMessageRecordRepository.GetListAsync()).ToList();
        query.Result = list.Map<List<DingtalkPushMessageRecordViewDto>>();
    }

    [EventHandler]
    public async Task GetPageHandleAsync(DingtalkPushMessageRecordPageQuery query)
    {
        var searchDto = query.SearchDto;
        var paginatedOptions = new PaginatedOptions
        {
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };
        var pageList = await _dingtalkPushMessageRecordRepository.GetPaginatedListAsync(paginatedOptions);
        var result = pageList.Result.Map<List<DingtalkPushMessageRecordViewDto>>();
        query.Result = new PaginatedListBase<DingtalkPushMessageRecordViewDto>
        {
            Result = result,
            Total = pageList.Total,
            TotalPages = pageList.TotalPages,
        };
    }
}