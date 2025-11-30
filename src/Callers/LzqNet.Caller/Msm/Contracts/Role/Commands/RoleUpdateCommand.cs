using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Role.Commands;
public record RoleUpdateCommand : Command
{
    public long Id { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
}
public class RoleUpdateCommandValidator : MasaAbstractValidator<RoleUpdateCommand>
{
    public RoleUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");

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