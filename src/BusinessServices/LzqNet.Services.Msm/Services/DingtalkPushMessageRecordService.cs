using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Commands;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class DingtalkPushMessageRecordService : ServiceBase
{
    public DingtalkPushMessageRecordService() : base("/api/v1/dingtalk/PushMessageRecord") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取钉钉推送消息记录分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取钉钉推送消息记录分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushMessageRecordPageSearchDto input)
    {
        var query = new DingtalkPushMessageRecordPageQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 获取钉钉推送消息记录列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取钉钉推送消息记录列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushMessageRecordSearchDto? input)
    {
        var query = new DingtalkPushMessageRecordListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 增加钉钉推送消息记录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加钉钉推送消息记录")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushMessageRecordCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新钉钉推送消息记录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新钉钉推送消息记录")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushMessageRecordUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除钉钉推送消息记录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除钉钉推送消息记录")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushMessageRecordDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除钉钉推送消息记录 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除钉钉推送消息记录")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushMessageRecordDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}