using LzqNet.Common.Callers.Auth;
using LzqNet.Common.Callers.Auth.Contracts;
using LzqNet.Extensions.Global;
using LzqNet.System.Contracts.Account.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.QueryHandlers;

public class AccountQueryHandler(IUserRepository userRepository, AuthCaller authCaller, IOptions<GlobalConfig> options, IHttpContextAccessor httpContextAccessor)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;
    private readonly GlobalConfig globalConfig = options.Value;
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    [EventHandler]
    public async Task GetUserInfoHandleAsync(UserInfoQuery query)
    {
        UserInfoViewDto? result;
        if (globalConfig.UseAuth)//微服务
            result = await _authCaller.UserInfo();
        else
        {
            result = GetCurrentUser();
        }
        if (result == null)
            throw new MasaException("获取用户信息失败");
        // 这里可以根据token得到的用户名信息查更多用户信息
        query.Result = result;
    }

    /// <summary>
    /// Claims获取当前用户信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    private UserInfoViewDto GetCurrentUser()
    {
        if (_httpContext == null)
            throw new MasaException("_httpContext为空");

        var user = _httpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
            throw new MasaException("用户未认证");

        // 从Claims中提取用户信息
        var userIdClaim = user.FindFirst("UserId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
        var userNameClaim = user.FindFirst("UserName") ?? user.FindFirst(ClaimTypes.Name);
        var rolesClaim = user.FindFirst("Roles");
        var emailClaim = user.FindFirst("Email") ?? user.FindFirst(ClaimTypes.Email);
        var sexClaim = user.FindFirst("Sex");

        if (userIdClaim == null || userNameClaim == null)
            throw new MasaException("用户信息不完整");

        // 解析角色信息
        List<string> roles = new List<string>();
        if (!string.IsNullOrEmpty(rolesClaim?.Value))
        {
            roles = JsonSerializer.Deserialize<List<string>>(rolesClaim.Value) ?? [];
        }

        // 构建返回的DTO
        var userInfoDto = new UserInfoViewDto
        {
            Id = long.Parse(userIdClaim.Value),
            UserName = userNameClaim.Value,
            Email = emailClaim?.Value ?? string.Empty,
            Roles = roles,
            Sex = int.TryParse(sexClaim?.Value, out int sex) ? sex : 0
        };

        return userInfoDto;
    }
}