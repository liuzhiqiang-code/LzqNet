using LzqNet.Common.Callers.Auth;
using LzqNet.Common.Callers.Auth.Contracts;
using LzqNet.Extensions.Global;
using LzqNet.Extensions.Jwt;
using LzqNet.Extensions.Jwt.Contracts;
using LzqNet.System.Contracts.Account.Commands;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using Microsoft.Extensions.Options;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.CommandHandlers;

public class AccountCommandHandler(IUserRepository userRepository,AuthCaller authCaller, AuthAppService authAppService, IOptions<GlobalConfig> options)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;
    private readonly AuthAppService _authAppService = authAppService;
    private readonly GlobalConfig globalConfig = options.Value;

    [EventHandler]
    public async Task LoginHandleAsync(LoginCommand command)
    {
        var user = await _userRepository.GetFirstAsync(a=>a.UserName.Equals(command.UserName));
        if (user == null)
            throw new MasaException("用户不存在");
        if (!user.Password.Equals(command.Password))
            throw new MasaException("密码错误");

        TokenViewDto? result;
        if (globalConfig.UseAuth)//微服务
            result = await _authCaller.Login(new UserLoginDto(command.UserName!, command.Password!));
        else
        {
            result = _authAppService.GenAuthenticate(user.Map<UserInfo>());
        }

        if (result == null)
            throw new MasaException("登录失败");
        command.Result = result;
    }

    [EventHandler]
    public async Task LogoutHandleAsync(LogoutCommand command)
    {
        if (globalConfig.UseAuth)//微服务
            await _authCaller.Logout();
    }
}