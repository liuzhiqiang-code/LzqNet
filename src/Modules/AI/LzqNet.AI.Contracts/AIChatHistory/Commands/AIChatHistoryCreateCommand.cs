using FluentValidation.Validators;
using LzqNet.AI.Domain.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChatHistory.Commands;

public record AIChatHistoryCreateCommand : Command
{
    /// <summary>
    /// AIChatsId
    /// </summary>
    public long? AIChatsId { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// MessageType
    /// </summary>
    public MessageTypeEnum? MessageType { get; set; }

}
public class AIChatHistoryCreateCommandValidator : MasaAbstractValidator<AIChatHistoryCreateCommand>
{
    public AIChatHistoryCreateCommandValidator()
    {
    }
}