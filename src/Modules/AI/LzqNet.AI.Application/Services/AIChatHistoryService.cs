using LzqNet.AI.Contracts.AIChatHistory.Commands;
using LzqNet.AI.Contracts.AIChatHistory.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LzqNet.AI.Application.Services;

public class AIChatHistoryService : ServiceBase
{
    public AIChatHistoryService() : base("/api/v1/aiChatHistory") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("AiChatHistory", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] AIChatHistoryPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("AiChatHistory", Description = "获取列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] AIChatHistoryListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("AiChatHistory", Description = "增加")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] AIChatHistoryCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChatHistory", Description = "更新")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] AIChatHistoryUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChatHistory", Description = "删除")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new AIChatHistoryDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChatHistory", Description = "批量删除")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new AIChatHistoryDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}