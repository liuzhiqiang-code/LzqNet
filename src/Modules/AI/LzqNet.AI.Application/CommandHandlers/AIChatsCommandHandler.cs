using LzqNet.AI.Contracts.AIChats.Commands;
using LzqNet.AI.Domain.Consts;
using LzqNet.AI.Domain.Entities;
using LzqNet.AI.Domain.Expands;
using LzqNet.AI.Domain.IRepositories;
using LzqNet.Extensions.AI.Interfaces;
using LzqNet.Extensions.Jwt;
using Masa.Contrib.Dispatcher.Events;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LzqNet.AI.Application.CommandHandlers;

public class AIChatsCommandHandler(IAIChatsRepository aiChatsRepository,
    IAIChatHistoryRepository aiChatHistoryRepository,
    IModelRunRecordRepository modelRunRecordRepository,
    ILogger<AIChatsCommandHandler> logger,
    IAIAgentService aIAgentService,
    ICurrentUser currentUser)
{
    private readonly IAIChatsRepository _aiChatsRepository = aiChatsRepository;
    private readonly IAIChatHistoryRepository _aiChatHistoryRepository = aiChatHistoryRepository;
    private readonly IModelRunRecordRepository _modelRunRecordRepository = modelRunRecordRepository;
    private readonly ILogger<AIChatsCommandHandler> _logger = logger;
    private readonly IAIAgentService _aIAgentService = aIAgentService;
    private readonly ICurrentUser _currentUser = currentUser;

    [EventHandler]
    public async Task CreateHandleAsync(AIChatsCreateCommand command)
    {
        var entity = command.Map<AIChatsEntity>();
        await _aiChatsRepository.InsertAsync(entity);
    }

    [EventHandler]
    public async Task UpdateHandleAsync(AIChatsUpdateCommand command)
    {
        var entity = command.Map<AIChatsEntity>();
        await _aiChatsRepository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteHandleAsync(AIChatsDeleteCommand command)
    {
        await _aiChatsRepository.DeleteAsync(a => command.Ids.Contains(a.Id));
    }


    [EventHandler]
    public async Task CompletionHandleAsync(AIChatsCompletionCommand command)
    {
        var stopwatch = Stopwatch.StartNew();
        ModelRunRecordEntity entity = new ModelRunRecordEntity()
        {
            ChatClient = command.ChatClient,
            AIAgentModel = command.AIAgentModel,
            AIAgentName = command.AIAgentModel.Name,
            Instructions = command.AIAgentModel.ChatOptions.Instructions ?? "",
            Prompt = command.Prompt,
            PromptTokens = command.Prompt.Length,
        };
        try
        {
            var (agent, content) = await _aIAgentService.CreateAIAgentAndRunStreamingAsync(command.ChatClient, command.AIAgentModel, command.Prompt, async (chunk) =>
            {
                await command.RaiseChunkReceivedAsync(chunk);
            });
            entity.IsSuccess = true;
            entity.Content = content;
            entity.CompletionTokens = content.Length;
        }
        catch (Exception ex)
        {
            entity.IsSuccess = false;
            entity.ErrorMessage = ex.Message;
            _logger.LogError("调用大模型失败，原因是：{Message}",ex.Message);
        }
        finally {
            stopwatch.Stop();
            entity.DurationMs = stopwatch.ElapsedMilliseconds;
            await _modelRunRecordRepository.InsertAsync(entity);
        }
        command.Result = entity.Map<AIChatContent>();
    }

    [EventHandler]
    public async Task TitleHandleAsync(AIChatsTitleCommand command)
    {
        // 生成主题的提示词，如果需要AI的结果什么的就都加上
        var prompt = command.Prompt;
        var chatClient = ChatClientConst.DeepSeekChat;
        var agent = AIAgentConst.TITLE;

        var stopwatch = Stopwatch.StartNew();
        ModelRunRecordEntity entity = new ModelRunRecordEntity()
        {
            ChatClient = chatClient,
            AIAgentModel = agent,
            AIAgentName = agent.Name,
            Instructions = agent.ChatOptions.Instructions ?? "",
            Prompt = command.Prompt,
            PromptTokens = command.Prompt.Length,
        };
        try
        {
            var (_, content) = await _aIAgentService.CreateAIAgentAndRunAsync(ChatClientConst.DeepSeekChat, AIAgentConst.TITLE, command.Prompt);
            command.Result = content;

            entity.IsSuccess = true;
            entity.Content = content;
            entity.CompletionTokens = content.Length;
        }
        catch (Exception ex)
        {
            entity.IsSuccess = false;
            entity.ErrorMessage = ex.Message;
            _logger.LogError("调用大模型失败，原因是：{Message}", ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            entity.DurationMs = stopwatch.ElapsedMilliseconds;
            await _modelRunRecordRepository.InsertAsync(entity);
        }
    }

    [EventHandler]
    public async Task SaveHandleAsync(AIChatsSaveCommand command)
    {
        if (command.AIChatsId.HasValue)
        {
            var entity = await _aiChatsRepository.GetFirstAsync(a=>a.Id.Equals(command.AIChatsId));

            AIChatHistoryEntity historyEntity = new AIChatHistoryEntity { };
            await _aiChatHistoryRepository.InsertAsync(historyEntity);
        }
        else {
            AIChatsEntity entity = new AIChatsEntity { };
            await _aiChatsRepository.InsertAsync(entity);

            AIChatHistoryEntity historyEntity = new AIChatHistoryEntity { };
            await _aiChatHistoryRepository.InsertAsync(historyEntity);
        }
    }
}