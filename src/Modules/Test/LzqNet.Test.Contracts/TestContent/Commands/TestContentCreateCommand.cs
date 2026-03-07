using FluentValidation.Validators;
using LzqNet.Common.Attributes;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContent.Commands;

[UnitOfWork]
public record TestContentCreateCommand : Command
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
public class TestContentCreateCommandValidator : MasaAbstractValidator<TestContentCreateCommand>
{
    public TestContentCreateCommandValidator()
    {
    }
}