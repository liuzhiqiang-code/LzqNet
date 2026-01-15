using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Modeling.Commands;

public record ModelingUpdateCommand : Command
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
    /// HasRevision
    /// </summary>
    public bool HasRevision { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

}
public class ModelingUpdateCommandValidator : MasaAbstractValidator<ModelingUpdateCommand>
{
    public ModelingUpdateCommandValidator()
    {
        RuleFor(x => x.ModelingId)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");
    }
}