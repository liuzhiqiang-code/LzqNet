using FluentValidation.Validators;
using LzqNet.Common.Attributes;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChats.Commands;

[UnitOfWork]
public record AIChatsSaveCommand : Command
{
    public long? AIChatsId { get; set; }

    public string ChatClient { get; set; }

    public string AIAgentName { get; set; }

    public string Title { get; set; }

    public string Instructions { get; set; }

    public string Prompt { get; set; }

    public string Content { get; set; }

    public int PromptTokens { get; set; }

    public int CompletionTokens { get; set; }
}
public class AIChatsSaveCommandValidator : MasaAbstractValidator<AIChatsSaveCommand>
{
    public AIChatsSaveCommandValidator()
    {
    }
}