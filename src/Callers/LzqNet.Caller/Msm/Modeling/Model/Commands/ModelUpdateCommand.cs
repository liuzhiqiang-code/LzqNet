using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Modeling.Model.Commands;

public record ModelUpdateCommand : Command
{
    /// <summary>
    /// ModelingId
    /// </summary>
    public long ModelingId { get; set; }

    /// <summary>
    /// ModelingName
    /// </summary>
    public string ModelingName { get; set; }

    /// <summary>
    /// TableName
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

}
public class ModelUpdateCommandValidator : MasaAbstractValidator<ModelUpdateCommand>
{
    public ModelUpdateCommandValidator()
    {
        RuleFor(x => x.ModelingId)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}