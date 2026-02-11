using LzqNet.System.Contracts.Dept;
using LzqNet.System.Contracts.Dept.Commands;
using LzqNet.System.Contracts.Dept.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.ComponentModel;

namespace LzqNet.System.Application.Services;

public class DeptService : ServiceBase
{
    public DeptService() : base("/api/v1/dept") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    [OpenApiTag("Dept", Description = "获取部门分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DeptPageQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(query.Result);
    }

    [OpenApiTag("Dept", Description = "获取部门列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DeptListQuery query)
    {
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    [OpenApiTag("Dept", Description = "增加部门")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DeptCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Dept", Description = "更新部门")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DeptUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Dept", Description = "删除部门")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DeptDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    [OpenApiTag("Dept", Description = "批量删除部门")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DeptDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}
