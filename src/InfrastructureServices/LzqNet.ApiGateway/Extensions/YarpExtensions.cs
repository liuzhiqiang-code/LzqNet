using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Health;

namespace LzqNet.ApiGateway.Extensions;

public static class YarpExtensions
{
    public static void AddCustomYarp(this IHostApplicationBuilder builder)
    {
        IReadOnlyList<RouteConfig> routeConfigs = builder.Configuration.GetSection("ReverseProxy:Routes")
            .Get<IReadOnlyList<RouteConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Routes");
        IReadOnlyList<ClusterConfig> clusterConfigs = builder.Configuration.GetSection("ReverseProxy:Clusters")
            .Get<IReadOnlyList<ClusterConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Clusters");

        // 验证反序列化结果类型
        if (routeConfigs?.Any() != true) throw new InvalidOperationException("路由配置不能为空");
        if (clusterConfigs?.Any() != true) throw new InvalidOperationException("集群配置不能为空");

        // 热更新配置提供器，在AddReverseProxy前执行
        builder.Services.AddSingleton<IHotReloadProxyConfigProvider, HotReloadProxyConfigProvider>();//热更新配置
        builder.Services.AddSingleton<IPassiveHealthCheckPolicy, CustomPassiveHealthCheckPolicy>();//被动健康检查策略
        // 1. 添加YARP服务
        builder.Services.AddReverseProxy()
            .LoadFromMemory(routeConfigs, clusterConfigs)
            .AddConfigFilter<CustomConfigFilter>() //注册配置过滤器
            //.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); // 从配置加载路由规则
            .ConfigureHttpClient((context, handler) => handler.ActivityHeadersPropagator = null);//穿透代理
    }
}
