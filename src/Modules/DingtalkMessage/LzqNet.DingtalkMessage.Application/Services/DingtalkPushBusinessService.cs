using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Commands;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LzqNet.DingtalkMessage.Application.Services;

public class DingtalkPushBusinessService : ServiceBase
{
    public DingtalkPushBusinessService() : base("/api/v1/dingtalk/PushBusiness") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("DingtalkPushBusiness", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushBusinessPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushBusiness", Description = "获取列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushBusinessListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushBusiness", Description = "增加")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushBusinessCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushBusiness", Description = "更新")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushBusinessUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushBusiness", Description = "删除")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushBusinessDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushBusiness", Description = "批量删除")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushBusinessDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}