using LzqNet.System.Contracts.Dept;
using LzqNet.System.Contracts.Dept.Commands;
using LzqNet.System.Contracts.Dept.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.System.Application.Services;

public class DeptService : ServiceBase
{
    public DeptService() : base("/api/v1/dept") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取部门分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取部门分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DeptPageSearchDto input)
    {
        var query = new DeptPageQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(query.Result);
    }

    /// <summary>
    /// 获取部门列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取部门列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DeptSearchDto? input)
    {
        var query = new DeptGetListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 增加部门 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加部门")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DeptCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新部门 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新部门")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DeptUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除部门 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除部门")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DeptDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除部门 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除部门")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DeptDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    ///// <summary>
    ///// 获取部门详情 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("获取部门详情")]
    //public async Task<DeptViewDto> GetDetail([FromQuery] DeptSearchDto input)
    //{
    //    return new DeptViewDto();
    //}

    ///// <summary>
    ///// 获取部门值
    ///// </summary>
    ///// <param name="code"></param>
    ///// <returns></returns>
    //[NonAction]
    //public async Task<T> GetConfigValue<T>(string code)
    //{
    //    return default(T);
    //}

    ///// <summary>
    ///// 更新部门值
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
    ///// 批量更新部门值
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("批量更新部门值")]
    //public async Task BatchUpdateConfig(List<UpdateDeptDto> input)
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
