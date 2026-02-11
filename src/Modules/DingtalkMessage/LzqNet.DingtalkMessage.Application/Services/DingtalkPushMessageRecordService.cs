using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Commands;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushMessageRecord.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LzqNet.DingtalkMessage.Application.Services;

public class DingtalkPushMessageRecordService : ServiceBase
{
    public DingtalkPushMessageRecordService() : base("/api/v1/dingtalk/PushMessageRecord") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("DingtalkPushMessageRecord", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushMessageRecordPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "获取钉钉推送消息记录列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushMessageRecordListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "增加钉钉推送消息记录")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushMessageRecordCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "发送钉钉推送消息")]
    [RoutePattern(pattern: "send", true)]
    public async Task<AdminResult> SendAsync([FromBody] DingtalkMessageSendCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "更新钉钉推送消息记录")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushMessageRecordUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "删除钉钉推送消息记录")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushMessageRecordDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushMessageRecord", Description = "批量删除钉钉推送消息记录")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushMessageRecordDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}