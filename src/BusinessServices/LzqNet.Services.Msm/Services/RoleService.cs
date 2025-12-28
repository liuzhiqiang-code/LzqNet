using LzqNet.Caller.Msm.Contracts.Role;
using LzqNet.Caller.Msm.Contracts.Role.Commands;
using LzqNet.Caller.Msm.Contracts.Role.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class RoleService : ServiceBase
{
    public RoleService() : base("/api/v1/role") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取角色分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取角色分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] RolePageSearchDto input)
    {
        var query = new RolePageQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 获取角色列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取角色列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] RoleSearchDto? input)
    {
        var query = new RoleGetListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 增加角色 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加角色")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] RoleCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新角色 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新角色")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] RoleUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除角色 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除角色")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new RoleDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除角色 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除角色")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new RoleDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}
