using LzqNet.System.Contracts.Menu;
using LzqNet.System.Contracts.Menu.Commands;
using LzqNet.System.Contracts.Menu.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.ComponentModel;

namespace LzqNet.System.Application.Services;

public class MenuService : ServiceBase
{
    public MenuService() : base("/api/v1/menu") { }
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("Menu", Description = "菜单名称是否存在")]
    [RoutePattern(pattern: "name-exists", true, HttpMethod="GET")]
    public async Task<IResult> NameExistsAsync([FromQuery] long? id, string? path)
    {
        var query = new MenuNameExistsQuery(id, path);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Menu", Description = "菜单路由是否存在")]
    [RoutePattern(pattern: "path-exists", true, HttpMethod = "GET")]
    public async Task<IResult> PathExistsAsync(long? id, string? path)
    {
        var query = new MenuPathExistsQuery(id, path);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Menu", Description = "获取菜单列表")]
    [RoutePattern(pattern: "list", true, HttpMethod = "GET")]
    public async Task<IResult> ListAsync([FromBody] MenuListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Menu", Description = "新增菜单")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] MenuCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Menu", Description = "更新菜单")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] MenuUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Menu", Description = "删除菜单")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new MenuDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Menu", Description = "批量删除菜单")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new MenuDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}
