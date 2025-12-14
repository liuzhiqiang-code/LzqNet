using LzqNet.Caller.Msm.Contracts.Menu;
using LzqNet.Caller.Msm.Contracts.Menu.Commands;
using LzqNet.Caller.Msm.Contracts.Menu.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class MenuService : ServiceBase
{
    public MenuService() : base("/api/v1/menu") { }
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 菜单名称是否存在 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("菜单名称是否存在")]
    [RoutePattern(pattern: "name-exists", true, HttpMethod="GET")]
    public async Task<IResult> NameExistsAsync([FromQuery] long? id, string? path)
    {
        var query = new MenuNameExistsQuery(id, path);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 菜单路由是否存在 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("菜单路由是否存在")]
    [RoutePattern(pattern: "path-exists", true, HttpMethod = "GET")]
    public async Task<IResult> PathExistsAsync(long? id, string? path)
    {
        var query = new MenuPathExistsQuery(id, path);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 获取菜单列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取菜单列表")]
    [RoutePattern(pattern: "list", true, HttpMethod = "GET")]
    public async Task<IResult> ListAsync([FromBody] MenuSearchDto? input)
    {
        var query = new MenuGetListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 新增菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("新增菜单")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] MenuCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新菜单")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] MenuUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除菜单")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new MenuDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除菜单 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除菜单")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new MenuDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}
