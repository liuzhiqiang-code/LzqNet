using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.AI.Contracts.AIChats.Commands;

public record AIChatsTitleCommand : Command
{
    public string Instructions { get; set; }

    public string Prompt { get; set; }

    public string Content { get; set; }

    public string Result { get; set; }
}
public class AIChatsTitleCommandValidator : MasaAbstractValidator<AIChatsTitleCommand>
{
    public AIChatsTitleCommandValidator()
    {
    }
}