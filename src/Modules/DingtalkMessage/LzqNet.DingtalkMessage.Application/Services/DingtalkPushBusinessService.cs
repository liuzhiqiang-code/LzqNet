using LzqNet.DingtalkMessage.Contracts.DingtalkPushBusiness;
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

    [OpenApiTag("钉钉推送", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushBusinessPageSearchDto input)
    {
        var query = new DingtalkPushBusinessPageQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("钉钉推送", Description = "获取列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushBusinessSearchDto? input)
    {
        var query = new DingtalkPushBusinessListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("钉钉推送", Description = "增加")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushBusinessCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("钉钉推送", Description = "更新")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushBusinessUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("钉钉推送", Description = "删除")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushBusinessDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("钉钉推送", Description = "批量删除")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushBusinessDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}