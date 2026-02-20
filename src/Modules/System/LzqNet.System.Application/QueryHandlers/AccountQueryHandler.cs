using LzqNet.Common.Callers.Auth;
using LzqNet.Common.Callers.Auth.Contracts;
using LzqNet.Extensions.Global;
using LzqNet.Extensions.Jwt;
using LzqNet.System.Contracts.Account.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;
using Microsoft.Extensions.Options;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.QueryHandlers;

public class AccountQueryHandler(IUserRepository userRepository,AuthCaller authCaller, AuthAppService authAppService,IOptions<GlobalConfig> options)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;
    private readonly AuthAppService _authAppService = authAppService;
    private readonly GlobalConfig globalConfig = options.Value;

    [EventHandler]
    public async Task GetUserInfoHandleAsync(UserInfoQuery query)
    {
        UserInfoViewDto? result;
        if (globalConfig.UseAuth)//微服务
            result = await _authCaller.UserInfo();
        else
        {
            result = _authAppService.GetCurrentUser();
        }
        if (result == null)
            throw new MasaException("获取用户信息失败");
        // 这里可以根据token得到的用户名信息查更多用户信息
        query.Result = result;
    }
}