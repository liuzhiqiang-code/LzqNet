using LzqNet.Common.Options;
using LzqNet.Extensions.Jwt;
using LzqNet.Extensions.Jwt.Callers;
using LzqNet.Extensions.Jwt.Callers.Contracts;
using LzqNet.Extensions.Jwt.Services;
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

public class AccountCommandHandler(IUserRepository userRepository, AuthCaller authCaller, IJwtService jwtService, IOptions<GlobalConfig> options)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;
    private readonly IJwtService _jwtService = jwtService;
    private readonly GlobalConfig globalConfig = options.Value;

    [EventHandler]
    public async Task LoginHandleAsync(LoginCommand command)
    {
        var user = await _userRepository.GetFirstAsync(a => a.UserName.Equals(command.UserName));
        if (user == null)
            throw new MasaException("用户不存在");
        if (!user.Password.Equals(command.Password))
            throw new MasaException("密码错误");

        TokenViewDto? result;
        if (globalConfig.UseAuth)//微服务
            result = await _authCaller.Login(new UserLoginDto(command.UserName!, command.Password!));
        else
        {
            ICurrentUser currentUser = new CurrentUser()
            .SetUserId(user.Id.ToString())
            .SetUserName(user.UserName)
            .SetEmail(user.Email)
            .SetSex(user.Sex.ToString()??"")
            .SetTenantId("")//暂时不管租户
            .SetRoles(user.Roles);

            result = _jwtService.GenerateToken(currentUser, TimeSpan.FromHours(2));
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