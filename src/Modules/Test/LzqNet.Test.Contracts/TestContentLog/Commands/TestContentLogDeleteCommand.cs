using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Test.Contracts.TestContentLog.Commands;

public record TestContentLogDeleteCommand : Command
{
    public List<long> Ids { get; set; }
    public TestContentLogDeleteCommand(List<long> ids)
    {
        Ids = ids;
    }
}
public class TestContentLogDeleteCommandValidator : MasaAbstractValidator<TestContentLogDeleteCommand>
{
    public TestContentLogDeleteCommandValidator()
    {
        RuleFor(x => x.Ids)
            .NotNull().WithMessage("ID列表不能为null")
            .NotEmpty().WithMessage("ID列表不能为空");
        // 每个ID必须大于0
        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage("ID必须大于0");
    }
}