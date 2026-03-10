using LzqNet.System.Contracts.Account.Commands;
using LzqNet.System.Contracts.Account.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.System.Application.Services;

public class AccountService : ServiceBase
{
    public AccountService() : base("/api/v1/account") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("Account", Description = "登录")]
    [RoutePattern(pattern: "login", true)]
    [AllowAnonymous]
    public async Task<IResult> LoginAsync([FromBody] LoginCommand command)
    {
        await EventBus.PublishAsync(command);
        return Results.Ok(AdminResult.Success(command.Result));
    }

    [OpenApiTag("Account", Description = "登出")]
    [RoutePattern(pattern: "logout", true)]
    [AllowAnonymous]
    public async Task<AdminResult> LogoutAsync()
    {
        await EventBus.PublishAsync(new LogoutCommand());
        return AdminResult.Success();
    }

    [OpenApiTag("Account", Description = "获取用户信息")]
    [RoutePattern(pattern: "userInfo", true, HttpMethod="Get")]
    public async Task<IResult> UserInfoAsync()
    {
        var query = new UserInfoQuery();
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }
}