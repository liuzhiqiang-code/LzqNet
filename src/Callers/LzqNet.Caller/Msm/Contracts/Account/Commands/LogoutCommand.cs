using FluentValidation;
using FluentValidation.Validators;
using LzqNet.Caller.Auth.Contracts;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Caller.Msm.Contracts.Account.Commands;

public record LogoutCommand : Command
{
   
}
public class LogoutCommandValidator : MasaAbstractValidator<LogoutCommand>
{
   
}