using FluentValidation.Validators;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Contracts.Account.Commands;

public record LogoutCommand : Command
{
   
}
public class LogoutCommandValidator : MasaAbstractValidator<LogoutCommand>
{
   
}