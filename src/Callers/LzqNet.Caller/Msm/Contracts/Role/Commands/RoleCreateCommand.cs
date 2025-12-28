using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Role.Commands;

public record RoleCreateCommand : Command
{
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
    public List<long> Permissions { get; set; } = [];
}
public class RoleCreateCommandValidator : MasaAbstractValidator<RoleCreateCommand>
{
    public RoleCreateCommandValidator()
    {
        RuleFor(a => a.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("角色名称不能为空");

        WhenNotEmpty(a => a.Remark,
            rule => rule
            .Length(0, 500)
            .WithMessage("备注信息不能超过500字符"));
    }
}