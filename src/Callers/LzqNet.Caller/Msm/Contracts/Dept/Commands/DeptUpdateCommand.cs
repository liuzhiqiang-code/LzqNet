using FluentValidation;
using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Msm.Contracts.Dept.Commands;
public record DeptUpdateCommand : Command
{
    public long Id { get; set; }
    public long? Pid { get; set; }
    public string DeptName { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
}
public class DeptUpdateCommandValidator : MasaAbstractValidator<DeptUpdateCommand>
{
    public DeptUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID必须大于0");

        RuleFor(a => a.DeptName)
            .NotNull()
            .NotEmpty()
            .WithMessage("部门名称不能为空");

        WhenNotEmpty(a => a.Remark,
            rule => rule
            .Length(500)
            .WithMessage("备注信息不能超过500字符"));
    }
}