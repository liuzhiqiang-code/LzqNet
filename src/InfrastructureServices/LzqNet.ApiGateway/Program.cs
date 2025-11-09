using LzqNet.ApiGateway;
using LzqNet.ApiGateway.Extensions;
using LzqNet.DCC;
using Yarp.ReverseProxy.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSerilog();
builder.AddApplicationConfiguration();

// Add services to the container.

// 全局限流中间件配置
builder.AddCustomRateLimiter("customPolicy");
builder.AddCustomYarp();
builder.Services.AddHttpContextAccessor();
builder.AddCustomMetrics();// 配置遥测中间件
builder.AddCustomResponseCaching();// 配置响应缓存中间件

builder.AddCustomAuthentication();
builder.AddCustomAuthorization("customPolicy");

var app = builder.Build();

app.UseCustomMetrics();// 使用遥测中间件

// 使用 CORS 中间件时，必须在 UseResponseCaching 之前调用 UseCors。
// app.UseCors();
app.UseRouting();
app.UseCustomResponseCaching();// 使用响应缓存中间件
app.UseCustomAuthentication();
app.UseCustomAuthorization();
// 全局限流中间件
app.UseRateLimiter();
// 2. 启用YARP中间件
app.MapReverseProxy(); // 必须放在路由映射之后

// 3. 基础路由示例
app.MapGet("/", () => "网关运行中 (YARP)").AllowAnonymous();


// 4. 更新路由配置API示例
// A示例：yarp利用IProxyConfigProvider调GetConfig方式热更新路由配置
app.MapPost("/reload/A", (IHotReloadProxyConfigProvider configProvider) =>
{
    IReadOnlyList<RouteConfig> routeConfigs = builder.Configuration.GetSection("ReverseProxy:Routes")
    .Get<IReadOnlyList<RouteConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Routes");
    IReadOnlyList<ClusterConfig> clusterConfigs = builder.Configuration.GetSection("ReverseProxy:Clusters")
        .Get<IReadOnlyList<ClusterConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Clusters");

    // 验证反序列化结果类型
    if (routeConfigs?.Any() != true) throw new InvalidOperationException("路由配置不能为空");
    if (clusterConfigs?.Any() != true) throw new InvalidOperationException("集群配置不能为空");
    var proxyConfig = new CustomProxyConfig([.. routeConfigs], [.. clusterConfigs]);
    configProvider.UpdateConfig(proxyConfig);
    return Results.Ok("配置已重新加载");
});
// B示例：yarp利用IProxyConfigProvider调GetConfig方式热更新路由配置
app.MapPost("/reload/B", (InMemoryConfigProvider inMemoryConfigProvider) =>
{
    IReadOnlyList<RouteConfig> routeConfigs = builder.Configuration.GetSection("ReverseProxy:Routes")
    .Get<IReadOnlyList<RouteConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Routes");
    IReadOnlyList<ClusterConfig> clusterConfigs = builder.Configuration.GetSection("ReverseProxy:Clusters")
        .Get<IReadOnlyList<ClusterConfig>>() ?? throw new InvalidOperationException($"未找到配置项:ReverseProxy:Clusters");

    // 验证反序列化结果类型
    if (routeConfigs?.Any() != true) throw new InvalidOperationException("路由配置不能为空");
    if (clusterConfigs?.Any() != true) throw new InvalidOperationException("集群配置不能为空");
    inMemoryConfigProvider.Update(routeConfigs, clusterConfigs);
    return Results.Ok("配置已重新加载");
});

app.MapGet("/default", () =>
{
    return "hello";
}).RequireAuthorization();// 将具有指定名称的授权策略添加到终结点。空 代表使用 default 策略
app.Run();