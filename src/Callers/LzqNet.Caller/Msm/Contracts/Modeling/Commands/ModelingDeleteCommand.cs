using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Modeling.Commands;

public record ModelingDeleteCommand : Command
{
    public List<long> Ids { get; set; }
    public ModelingDeleteCommand(List<long> ids)
    {
        Ids = ids;
    }
}
public class ModelingDeleteCommandValidator : MasaAbstractValidator<ModelingDeleteCommand>
{
    public ModelingDeleteCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotNull().WithMessage("ID列表不能为null")
            .NotEmpty().WithMessage("ID列表不能为空");
        // 每个ID必须大于0
        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage("ID必须大于0");
    }
}