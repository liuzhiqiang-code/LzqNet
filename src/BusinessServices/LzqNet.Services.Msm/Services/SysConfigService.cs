using LzqNet.Caller.Msm.Contracts.SysConfig;
using LzqNet.Caller.Msm.Contracts.SysConfig.Commands;
using LzqNet.Caller.Msm.Contracts.SysConfig.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class SysConfigService : ServiceBase
{
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取参数配置分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置分页列表")]
    public async Task<IResult> Page(SysConfigSearchDto input)
    {
        var sysConfigPageQuery = new SysConfigPageQuery(input);
        await EventBus.PublishAsync(sysConfigPageQuery);
        return Results.Ok(sysConfigPageQuery.Result);
    }

    /// <summary>
    /// 获取参数配置列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取参数配置列表")]
    public async Task<IResult> List(SysConfigSearchDto input)
    {
        var sysConfigPageQuery = new SysConfigGetListQuery(input);
        await EventBus.PublishAsync(sysConfigPageQuery);
        return Results.Ok(sysConfigPageQuery.Result);
    }

    /// <summary>
    /// 增加参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加参数配置")]
    public async Task AddConfig(SysConfigCreateCommand command)
    {
        await EventBus.PublishAsync(command);
    }

    /// <summary>
    /// 更新参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新参数配置")]
    public async Task UpdateConfig(SysConfigUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
    }

    /// <summary>
    /// 删除参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除参数配置")]
    public async Task DeleteConfig(long id)
    {
        var command = new SysConfigDeleteCommand([id]);
        await EventBus.PublishAsync(command);
    }

    /// <summary>
    /// 批量删除参数配置 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [DisplayName("批量删除参数配置")]
    public async Task BatchDeleteConfig(List<long> ids)
    {
        var command = new SysConfigDeleteCommand(ids);
        await EventBus.PublishAsync(command);
    }

    ///// <summary>
    ///// 获取参数配置详情 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("获取参数配置详情")]
    //public async Task<SysConfigViewDto> GetDetail([FromQuery] SysConfigSearchDto input)
    //{
    //    return new SysConfigViewDto();
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
    //public async Task BatchUpdateConfig(List<UpdateSysConfigDto> input)
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
