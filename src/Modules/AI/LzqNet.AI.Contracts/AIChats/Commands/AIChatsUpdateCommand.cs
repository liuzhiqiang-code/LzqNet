using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChats.Commands;

public record AIChatsUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// AIChatsName
    /// </summary>
    public string? AIChatsName { get; set; }

    /// <summary>
    /// LastMessage
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public long? UserId { get; set; }

}
public class AIChatsUpdateCommandValidator : MasaAbstractValidator<AIChatsUpdateCommand>
{
    public AIChatsUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}