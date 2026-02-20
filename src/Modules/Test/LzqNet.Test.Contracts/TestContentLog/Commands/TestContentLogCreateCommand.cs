using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContentLog.Commands;

public record TestContentLogCreateCommand : Command
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
public class TestContentLogCreateCommandValidator : MasaAbstractValidator<TestContentLogCreateCommand>
{
    public TestContentLogCreateCommandValidator()
    {
    }
}