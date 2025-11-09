using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Contracts.Msm.SysConfig.Commands;
public record SysConfigCreateCommand : Command
{

}
public class SysConfigCreateCommandValidator : MasaAbstractValidator<SysConfigCreateCommand>
{
    public SysConfigCreateCommandValidator()
    {
        //RuleFor(user => user.Account).Letter();

        // WhenNotEmpty 的调用示例
        //_ = WhenNotEmpty(r => r.Phone, r => r.Phone, new PhoneValidator<RegisterUser>());
        //_ = WhenNotEmpty(r => r.Phone, new PhoneValidator<RegisterUser>());
        //_ = WhenNotEmpty(r => r.Phone, r => r.Phone, rule => rule.Phone());
        //_ = WhenNotEmpty(r => r.Phone, rule => rule.Phone());
    }
}