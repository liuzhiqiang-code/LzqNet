using LzqNet.Common.Options;
using LzqNet.Common.Utils;
using LzqNet.Extensions.Jwt;
using LzqNet.Extensions.Jwt.Callers;
using LzqNet.Extensions.Jwt.Callers.Contracts;
using LzqNet.System.Contracts.Account.Queries;
using Masa.Contrib.Dispatcher.Events;
using Microsoft.Extensions.Options;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.QueryHandlers;

public class AccountQueryHandler(AuthCaller authCaller, IOptions<GlobalConfig> options, ICurrentUser currentUser)
{
    private readonly AuthCaller _authCaller = authCaller;
    private readonly GlobalConfig globalConfig = options.Value;
    private readonly ICurrentUser _currentUser = currentUser;

    [EventHandler]
    public async Task GetUserInfoHandleAsync(UserInfoQuery query)
    {
        UserInfoViewDto? result;
        if (globalConfig.UseAuth)//微服务
            result = await _authCaller.UserInfo();
        else
        {
            result = new UserInfoViewDto
            {
                Id = _currentUser.UserId.ToInt64(),
                UserName = _currentUser.UserName,
                Email = _currentUser.Email,
            };
        }
        if (result == null)
            throw new MasaException("获取用户信息失败");
        // 这里可以根据token得到的用户名信息查更多用户信息
        query.Result = result;
    }
}