using LzqNet.Caller.Msm.Modeling.Model;
using LzqNet.Caller.Msm.Modeling.Model.Commands;
using LzqNet.Caller.Msm.Modeling.Model.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services.Modeling;

public class ModelService : ServiceBase
{
    public ModelService() : base("/api/v1/modeling/Model/") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取Id/Name的下拉框数据列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取Id/Name的下拉框数据列表")]
    [RoutePattern(pattern: "selectlist", true, HttpMethod = "Post")]
    public async Task<IResult> SelectListAsync([FromBody] ModelSearchDto input)
    {
        var query = new ModelSelectListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 根据Id获取对应数据详情 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取模型列表")]
    [RoutePattern(pattern: "/data", true, HttpMethod = "Get")]
    public async Task<IResult> DataAsync([FromQuery] long id)
    {
        var query = new ModelDataQuery(id);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }

    /// <summary>
    /// 增加数据 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加数据")]
    [RoutePattern(pattern: "create", true)]
    public async Task<AdminResult> CreateAsync([FromBody] ModelCreateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 更新数据 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新数据")]
    [RoutePattern(pattern: "update", true)]
    public async Task<AdminResult> UpdateAsync([FromBody] ModelUpdateCommand command)
    {
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }

    /// <summary>
    /// 删除数据 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除数据")]
    [RoutePattern(pattern: "delete/{id}", true)]
    public async Task<AdminResult> DeleteAsync(long id)
    {
        var command = new ModelDeleteCommand([id]);
        await EventBus.PublishAsync(command);
        return AdminResult.Success();
    }
}