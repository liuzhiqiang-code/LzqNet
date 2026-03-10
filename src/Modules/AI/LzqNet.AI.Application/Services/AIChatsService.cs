using LzqNet.AI.Contracts.AIChats;
using LzqNet.AI.Contracts.AIChats.Commands;
using LzqNet.AI.Contracts.AIChats.Queries;
using LzqNet.AI.Domain.Consts;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using OpenAI.Chat;
using SqlSugar;

namespace LzqNet.AI.Application.Services;


public class AIChatsService : ServiceBase
{
    public AIChatsService() : base("/api/v1/aiChats") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();
    private HttpContext httpContext => GetRequiredService<IHttpContextAccessor>().HttpContext!;


    [OpenApiTag("AiChats", Description = "获取分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] AIChatsPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("AiChats", Description = "获取列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] AIChatsListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("AiChats", Description = "增加")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] AIChatsCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChats", Description = "更新")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] AIChatsUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChats", Description = "删除")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new AIChatsDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChats", Description = "批量删除")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new AIChatsDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("AiChats", Description = "对话补全")]
    [RoutePattern(pattern: "completion", true)]
    public async Task CompletionAsync([FromBody] AIChatsCompletionRequest input)
    {
        // * 1. 设置SSE响应头
        httpContext.Response.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";
        httpContext.Response.Headers["X-Accel-Buffering"] = "no"; // 禁用代理服务器缓冲

        try
        {
            // * 2. 接收请求，获取模型信息、智能体信息、对话信息
            // var chatClient = ChatClientConst.DeepSeekChat;
            var chatClient = input.ChatClient;
            AIAgentModel aIAgentModel = AIAgentConst.CHAT;

            // * 3. 通过SSE流式返回AI的响应内容   AI响应完成后推送批处理信息
            AIChatsCompletionCommand command = new AIChatsCompletionCommand()
            {
                AIAgentModel = aIAgentModel,
                ChatClient = chatClient,
                Prompt = input.Prompt
            };
            // 订阅流式事件
            command.OnChunkReceivedAsync += async (chunk) =>
            {
                await WriteSseEventAsync("message", new { v = chunk });
            };
            await EventBus.PublishAsync(command);
            var completionResult = command.Result;

            // * 4. AI响应完成后推送批处理信息
            var batchData = new
            {
                p = "response",
                o = "BATCH",
                v = new object[]
                {
                    new { p = "accumulated_token_usage", v = completionResult.CompletionTokens },
                    new { p = "quasi_status", v = "FINISHED" }
                }
            };
            await WriteSseEventAsync("message", batchData);

            // 推送状态更新
            await WriteSseEventAsync("message", new
            {
                p = "response/status",
                o = "SET",
                v = "FINISHED"
            });

            // * 4. 生成消息主题的事件
            AIChatsTitleCommand titleCommand = new AIChatsTitleCommand()
            {
                Instructions = completionResult.Instructions,
                Prompt = completionResult.Prompt,
                Content = completionResult.Content
            };
            await EventBus.PublishAsync(titleCommand);
            var title = titleCommand.Result ?? "新对话";
            await WriteSseEventAsync("title", new { content = title });

            // * 5. 异步将完整对话信息和对话记录保存到数据库
            AIChatsSaveCommand saveCommand = new AIChatsSaveCommand()
            {
                AIChatsId = input.AIChatsId,
                ChatClient = chatClient,
                AIAgentName = aIAgentModel.Name,
                Title = title,
                Instructions = completionResult.Instructions,
                Prompt = completionResult.Prompt,
                Content = completionResult.Content,
                PromptTokens = completionResult.PromptTokens,
                CompletionTokens = completionResult.CompletionTokens,
            };
            _ = EventBus.PublishAsync(saveCommand);

            await WriteSseEventAsync("update_session", new { updated_at = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
            await WriteSseEventAsync("close", new { click_behavior = "none", auto_resume = false });
        }
        catch (Exception ex)
        {
            // 错误处理
            await WriteSseEventAsync("message", new { v = ex.Message });
        }
        finally {
            // 确保响应流关闭
            await httpContext.Response.CompleteAsync();
        }
    }

    private async Task WriteSseEventAsync(string eventName, object data)
    {
        // 将对象序列化为标准 JSON：{"v": "你好"}
        var jsonString = System.Text.Json.JsonSerializer.Serialize(data);
        await httpContext.Response.WriteAsync($"{eventName}: {jsonString}\n\n");
        await httpContext.Response.Body.FlushAsync();
    }
}