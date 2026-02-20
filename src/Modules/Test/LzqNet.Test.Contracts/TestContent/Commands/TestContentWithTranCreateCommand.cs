using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContent.Commands;

public record TestContentWithTranCreateCommand : Command
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
public class TestContentWithTranCreateCommandValidator : MasaAbstractValidator<TestContentWithTranCreateCommand>
{
    public TestContentWithTranCreateCommandValidator()
    {
    }
}