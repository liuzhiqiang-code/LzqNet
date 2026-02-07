using Masa.Contrib.Dispatcher.Events;
using LzqNet.Caller.Auth;
using LzqNet.System.Contracts.Account.Commands;
using LzqNet.System.Domain.IRepositories;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.CommandHandlers;

public class AccountCommandHandler(IUserRepository userRepository,AuthCaller authCaller)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;

    [EventHandler]
    public async Task LoginHandleAsync(LoginCommand command)
    {
        var result = await _authCaller.Login(new UserLoginDto(command.UserName!,command.Password!));
        if (result == null)
            throw new MasaException("登录失败");
        command.Result = result;
    }

    [EventHandler]
    public async Task LogoutHandleAsync(LogoutCommand command)
    {
        await _authCaller.Logout();
    }
}