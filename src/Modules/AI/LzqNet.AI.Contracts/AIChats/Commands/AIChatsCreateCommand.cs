using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChats.Commands;

public record AIChatsCreateCommand : Command
{
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
public class AIChatsCreateCommandValidator : MasaAbstractValidator<AIChatsCreateCommand>
{
    public AIChatsCreateCommandValidator()
    {
    }
}