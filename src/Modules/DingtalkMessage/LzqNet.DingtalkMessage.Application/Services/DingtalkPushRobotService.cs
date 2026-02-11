using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot.Commands;
using LzqNet.DingtalkMessage.Contracts.DingtalkPushRobot.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LzqNet.DingtalkMessage.Application.Services;

public class DingtalkPushRobotService : ServiceBase
{
    public DingtalkPushRobotService() : base("/api/v1/dingtalk/PushRobot") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("DingtalkPushRobot", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DingtalkPushRobotPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushRobot", Description = "获取钉钉推送机器人列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DingtalkPushRobotListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("DingtalkPushRobot", Description = "增加钉钉推送机器人")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DingtalkPushRobotCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushRobot", Description = "更新钉钉推送机器人")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DingtalkPushRobotUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushRobot", Description = "删除钉钉推送机器人")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DingtalkPushRobotDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("DingtalkPushRobot", Description = "批量删除钉钉推送机器人")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DingtalkPushRobotDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}