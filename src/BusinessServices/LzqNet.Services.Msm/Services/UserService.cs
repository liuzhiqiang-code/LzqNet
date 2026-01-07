using LzqNet.Caller.Msm.Contracts.User;
using LzqNet.Caller.Msm.Contracts.User.Commands;
using LzqNet.Caller.Msm.Contracts.User.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Services.Msm.Services;

public class UserService : ServiceBase
{
    public UserService() : base("/api/v1/user") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取用户分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取用户分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] UserPageSearchDto input)
    {
        var query = new UserPageQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 获取用户列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取用户列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] UserSearchDto? input)
    {
        var query = new UserGetListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 增加用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加用户")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] UserCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新用户")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] UserUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除用户")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new UserDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除用户 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除用户")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new UserDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}