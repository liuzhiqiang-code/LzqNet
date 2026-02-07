using LzqNet.Caller.Auth;
using LzqNet.System.Contracts.Account.Queries;
using LzqNet.System.Domain.IRepositories;
using Masa.Contrib.Dispatcher.Events;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.QueryHandlers;

public class AccountQueryHandler(IUserRepository userRepository,AuthCaller authCaller)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthCaller _authCaller = authCaller;

    [EventHandler]
    public async Task GetUserInfoHandleAsync(UserInfoQuery query)
    {
        var result = await _authCaller.UserInfo();
        if (result == null)
            throw new MasaException("获取用户信息失败");
        // 这里可以根据token得到的用户名信息查更多用户信息
        query.Result = result;
    }
}