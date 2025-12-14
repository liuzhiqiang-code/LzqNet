using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

namespace LzqNet.Caller.Msm.Contracts.Menu.Commands;

public record MenuCreateCommand : Command
{
    public long? Pid { get; set; }
    public string? AuthCode { get; set; }
    public string? Component { get; set; }
    public MenuMeta? Meta { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Path { get; set; }
    public string? Redirect { get; set; }
    public string Type { get; set; }
}
public class MenuCreateCommandValidator : MasaAbstractValidator<MenuCreateCommand>
{
    public MenuCreateCommandValidator()
    {
        RuleFor(a => a.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("菜单名称不能为空");

        RuleFor(a => a.Path)
            .NotEmpty()
            .When(a => a.Type == MenuType.Button.GetDescriptionValue())
            .WithMessage("菜单类型是按钮时，路径不能为空");

        RuleFor(a => a.Type)
           .NotNull()
           .NotEmpty()
           .WithMessage("菜单类型不能为空");
    }
}