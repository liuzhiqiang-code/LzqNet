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
        return Results.Ok(query.Result);
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

    ///// <summary>
    ///// 获取角色详情 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("获取角色详情")]
    //public async Task<RoleViewDto> GetDetail([FromQuery] RoleSearchDto input)
    //{
    //    return new RoleViewDto();
    //}

    ///// <summary>
    ///// 获取角色值
    ///// </summary>
    ///// <param name="code"></param>
    ///// <returns></returns>
    //[NonAction]
    //public async Task<T> GetConfigValue<T>(string code)
    //{
    //    return default(T);
    //}

    ///// <summary>
    ///// 更新角色值
    ///// </summary>
    ///// <param name="code"></param>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //[NonAction]
    //public async Task UpdateConfigValue(string code, string value)
    //{

    //}

    ///// <summary>
    ///// 获取分组列表 🔖
    ///// </summary>
    ///// <returns></returns>
    //[DisplayName("获取分组列表")]
    //public async Task<List<string>> GetGroupList()
    //{
    //    return new List<string>();
    //}

    ///// <summary>
    ///// 获取 Token 过期时间
    ///// </summary>
    ///// <returns></returns>
    //[NonAction]
    //public async Task<int> GetTokenExpire()
    //{
    //    return 0;
    //}

    ///// <summary>
    ///// 获取 RefreshToken 过期时间
    ///// </summary>
    ///// <returns></returns>
    //[NonAction]
    //public async Task<int> GetRefreshTokenExpire()
    //{
    //    return 0;
    //}

    ///// <summary>
    ///// 批量更新角色值
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("批量更新角色值")]
    //public async Task BatchUpdateConfig(List<UpdateRoleDto> input)
    //{

    //}

    ///// <summary>
    ///// 获取系统信息 🔖
    ///// </summary>
    ///// <returns></returns>
    //[AllowAnonymous]
    //[DisplayName("获取系统信息")]
    //public async Task<dynamic> GetSysInfo()
    //{
    //    return new
    //    {
    //        SystemName = "LzqNet",
    //        Version = "1.0.0",
    //        Description = "Masa Manufacturing Execution System"
    //    };
    //}
}
