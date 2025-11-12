using LzqNet.ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Configuration;

namespace LzqNet.ApiGateway.Services;

public static class YarpApi
{
    public static RouteGroupBuilder MapYarpApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api");

        api.MapGet("/config", GetProxyConfigAsync);
        api.MapPost("/reload", ReloadAsync);

        //// 4. 更新路由配置API示例
        //// A示例：yarp利用IProxyConfigProvider调GetConfig方式热更新路由配置
        //app.MapPost("/reload/A", (IHotReloadProxyConfigProvider configProvider, [FromBody] CustomProxyConfig proxyConfig) =>
        //{
        //    configProvider.UpdateConfig(proxyConfig);
        //    return Results.Ok("配置已重新加载");
        //});

        return api;
    }

    public static IResult GetProxyConfigAsync(IProxyConfigProvider configProvider)
    {
        try
        {
            var config = configProvider.GetConfig();
            if (config == null)
            {
                return Results.Problem("未能获取到Yarp配置，配置为空");
            }
            return Results.Ok(config.Map<ProxyConfigModel>());
        }
        catch (Exception ex)
        {
            return Results.Problem($"获取Yarp配置时发生异常: {ex.Message}");
        }
    }

    public static IResult ReloadAsync(InMemoryConfigProvider inMemoryConfigProvider, [FromBody] ProxyConfigModel proxyConfig)
    {
        IReadOnlyList<RouteConfig> routeConfigs = proxyConfig.Routes.Map<IReadOnlyList<RouteConfig>>();
        IReadOnlyList<ClusterConfig> clusterConfigs = proxyConfig.Clusters.Map<IReadOnlyList<ClusterConfig>>();

        // 验证反序列化结果类型
        if (routeConfigs?.Any() != true) throw new InvalidOperationException("路由配置不能为空");
        if (clusterConfigs?.Any() != true) throw new InvalidOperationException("集群配置不能为空");
        inMemoryConfigProvider.Update(routeConfigs, clusterConfigs);
        return Results.Ok("配置已重新加载");
    }
}
