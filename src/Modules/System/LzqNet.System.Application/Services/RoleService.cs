using LzqNet.System.Contracts.Role;
using LzqNet.System.Contracts.Role.Commands;
using LzqNet.System.Contracts.Role.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.ComponentModel;

namespace LzqNet.System.Application.Services;

public class RoleService : ServiceBase
{
    public RoleService() : base("/api/v1/role") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("Role", Description = "获取角色分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] RolePageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Role", Description = "获取角色列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] RoleListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Role", Description = "增加角色")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] RoleCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Role", Description = "更新角色")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] RoleUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Role", Description = "删除角色")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new RoleDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Role", Description = "批量删除角色")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new RoleDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}
