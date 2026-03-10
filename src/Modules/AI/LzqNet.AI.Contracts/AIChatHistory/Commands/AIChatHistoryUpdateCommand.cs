using FluentValidation;
using FluentValidation.Validators;
using LzqNet.AI.Domain.Enums;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChatHistory.Commands;

public record AIChatHistoryUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

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
public class AIChatHistoryUpdateCommandValidator : MasaAbstractValidator<AIChatHistoryUpdateCommand>
{
    public AIChatHistoryUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}