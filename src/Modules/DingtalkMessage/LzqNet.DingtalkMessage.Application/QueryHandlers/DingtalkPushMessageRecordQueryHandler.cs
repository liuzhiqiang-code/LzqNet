using LzqNet.Common.Contracts;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Queries;
using LzqNet.DingtalkMessage.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.DingtalkMessage.Application.QueryHandlers;

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
        RefAsync<int> total = 0;
        var pageList = await _dingtalkPushMessageRecordRepository.AsQueryable().ToPageListAsync(query.Page, query.PageSize, total);
        var result = pageList.Map<List<DingtalkPushMessageRecordViewDto>>();
        query.Result = new PageList<DingtalkPushMessageRecordViewDto>(result, total);
    }
}