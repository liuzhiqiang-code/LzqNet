using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Commands;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.ComponentModel;

namespace LzqNet.DingtalkMessage.Application.Services;

public class DingtalkPushConfigService : ServiceBase
{
    public DingtalkPushConfigService() : base("/api/v1/dingtalk/PushConfig") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("DingtalkPushConfig", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushConfigPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushConfig", Description = "获取钉钉推送配置列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushConfigListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushConfig", Description = "增加钉钉推送配置")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushConfigCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushConfig", Description = "更新钉钉推送配置")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushConfigUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushConfig", Description = "删除钉钉推送配置")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushConfigDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushConfig", Description = "批量删除钉钉推送配置")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushConfigDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}