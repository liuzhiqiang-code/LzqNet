using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContent.Commands;

public record TestContentUpdateCommand : Command
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

}
public class TestContentUpdateCommandValidator : MasaAbstractValidator<TestContentUpdateCommand>
{
    public TestContentUpdateCommandValidator()
    {
        //RuleFor(x => x.Id)
        //    .GreaterThan(0)
        //    .WithMessage("ID必须大于0");
    }
}