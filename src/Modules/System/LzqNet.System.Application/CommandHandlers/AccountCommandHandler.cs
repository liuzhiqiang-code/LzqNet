using LzqNet.Common.Callers.Auth;
using LzqNet.Common.Callers.Auth.Contracts;
using LzqNet.Extensions.Global;
using LzqNet.System.Contracts.Account.Commands;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using Masa.Utils.Security.Token;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.CommandHandlers;

public class AccountCommandHandler(IUserRepository userRepository,AuthCaller authCaller, IOptions<GlobalConfig> options)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;
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
            var claim = new Claim[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName),
                new Claim("Roles", user.Roles.ToJson()),
                new Claim("Email", user.Email ?? ""),
                new Claim("Sex", user.Sex.ToString() ?? ""),
                new Claim("datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim("token_type", "access"), // 标记Token类型
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()) // Token唯一标识
            };
            var accessToken = JwtUtils.CreateToken(claim, TimeSpan.FromHours(2));
            result = new TokenViewDto
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresIn = TimeSpan.FromHours(2).Milliseconds,
            };
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