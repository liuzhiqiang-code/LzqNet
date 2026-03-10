using FluentValidation.Validators;
using LzqNet.AI.Domain.Expands;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChats.Commands;

public record AIChatsCompletionCommand : Command
{
    /// <summary>
    /// ChatClient
    /// </summary>
    public string ChatClient { get; set; }

    /// <summary>
    /// AIAgentModel
    /// </summary>
    public AIAgentModel AIAgentModel { get; set; }

    /// <summary>
    /// Prompt
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// Result
    /// </summary>
    public AIChatContent Result { get; set; }

    /// <summary>
    /// 订阅流式事件
    /// </summary>
    public event Func<string, Task> OnChunkReceivedAsync;

    public async Task RaiseChunkReceivedAsync(string chunk)
    {
        if (OnChunkReceivedAsync != null)
        {
            await OnChunkReceivedAsync.Invoke(chunk);
        }
    }
}
public class AIChatsCompletionCommandValidator : MasaAbstractValidator<AIChatsCompletionCommand>
{
    public AIChatsCompletionCommandValidator()
    {
    }
}