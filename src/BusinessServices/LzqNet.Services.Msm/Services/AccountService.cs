using LzqNet.Caller.Msm.Contracts.Account.Commands;
using LzqNet.Caller.Msm.Contracts.Account.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Services.Msm.Services;

public class AccountService : ServiceBase
{
    public AccountService() : base("/api/v1/account") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 登录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("登录")]
    [RoutePattern(pattern: "login", true)]
    [AllowAnonymous]
    public async Task<IResult> LoginAsync([FromBody] LoginCommand command)
    {
        await EventBus.PublishAsync(command);
        return Results.Ok(AdminResult.Success(command.Result));
    }

    /// <summary>
    /// 登出 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("登出")]
    [RoutePattern(pattern: "logout", true)]
    [AllowAnonymous]
    public async Task<AdminResult> LogoutAsync()
    {
        await EventBus.PublishAsync(new LogoutCommand());
        return AdminResult.Success();
    }

    /// <summary>
    /// 获取用户信息 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取用户信息")]
    [RoutePattern(pattern: "userInfo", true, HttpMethod="Get")]
    public async Task<IResult> UserInfoAsync()
    {
        var query = new UserInfoQuery();
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }
}