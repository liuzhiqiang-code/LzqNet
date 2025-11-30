using LzqNet.Caller.Msm.Contracts.Dept;
using LzqNet.Caller.Msm.Contracts.Dept.Commands;
using LzqNet.Caller.Msm.Contracts.Dept.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class DeptService : ServiceBase
{
    public DeptService() : base("/api/v1/dept") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取参数配置分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置分页列表")]
    [RoutePattern(pattern: "page", true)]
    public async Task<IResult> PageAsync([FromBody] DeptPageSearchDto input)
    {
        var DeptPageQuery = new DeptPageQuery(input);
        await EventBus.PublishAsync(DeptPageQuery);
        return Results.Ok(DeptPageQuery.Result);
    }

    /// <summary>
    /// 获取参数配置列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取参数配置列表")]
    [RoutePattern(pattern: "list", true)]
    public async Task<IResult> ListAsync([FromBody] DeptSearchDto? input)
    {
        var DeptPageQuery = new DeptGetListQuery(input);
        await EventBus.PublishAsync(DeptPageQuery);
        return Results.Ok(AdminResult.Success(DeptPageQuery.Result));
    }

    /// <summary>
    /// 增加参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加参数配置")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] DeptCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新参数配置")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] DeptUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除参数配置")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new DeptDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 批量删除参数配置 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除参数配置")]
    [RoutePattern(pattern: "batchDelete", true, HttpMethod = "Delete")]
    public async Task<AdminResult> BatchDeleteAsync([FromBody] List<long> ids)
    {
        var command = new DeptDeleteCommand(ids);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    ///// <summary>
    ///// 获取参数配置详情 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("获取参数配置详情")]
    //public async Task<DeptViewDto> GetDetail([FromQuery] DeptSearchDto input)
    //{
    //    return new DeptViewDto();
    //}

    ///// <summary>
    ///// 获取参数配置值
    ///// </summary>
    ///// <param name="code"></param>
    ///// <returns></returns>
    //[NonAction]
    //public async Task<T> GetConfigValue<T>(string code)
    //{
    //    return default(T);
    //}

    ///// <summary>
    ///// 更新参数配置值
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
    ///// 批量更新参数配置值
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("批量更新参数配置值")]
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
