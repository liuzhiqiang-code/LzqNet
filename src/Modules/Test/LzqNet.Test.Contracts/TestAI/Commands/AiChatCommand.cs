using FluentValidation.Validators;
using LzqNet.Common.Attributes;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContent.Commands;

[UnitOfWork]
public record AiChatCommand : Command
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

}
public class AiChatCommandValidator : MasaAbstractValidator<AiChatCommand>
{
    public AiChatCommandValidator()
    {
    }
}