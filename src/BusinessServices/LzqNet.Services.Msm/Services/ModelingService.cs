using LzqNet.Caller.Msm.Contracts.Modeling;
using LzqNet.Caller.Msm.Contracts.Modeling.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Msm.Services;

public class ModelingService : ServiceBase
{
    public ModelingService() : base("/api/v1/modeling") { }

    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// 获取模型下拉列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取模型下拉列表")]
    [RoutePattern(pattern: "selectlist", true, HttpMethod = "Post")]
    public async Task<IResult> SelectListAsync([FromBody] ModelingSearchDto? input)
    {
        var query = new ModelingSelectListQuery(input);
        await EventBus.PublishAsync(query);
        return Results.Ok(AdminResult.Success(query.Result));
    }
}