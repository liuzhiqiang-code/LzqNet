using LzqNet.ApiGateway.Metrics;

namespace LzqNet.ApiGateway.Extensions;

public static class MetricsExtensions
{
    /// <summary>
    /// 客制化遥测中间件
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyName"></param>
    public static void AddCustomMetrics(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // 用于收集有关代理转发的常规指标的接口
        services.AddMetricsConsumer<ForwarderMetricsConsumer>();

        // 将使用者注册到代理转发器遥测的事件
        services.AddTelemetryConsumer<ForwarderTelemetryConsumer>();

        // 将使用者注册到HttpClient遥测事件
        services.AddTelemetryConsumer<HttpClientTelemetryConsumer>();

        services.AddTelemetryConsumer<WebSocketsTelemetryConsumer>();

    }

    public static void UseCustomMetrics(this WebApplication app)
    {
        // 收集和报告代理度量的自定义中间件
        // 放置在开头，因此它是每个请求运行的第一件也是最后一件事
        app.UseMiddleware<PerRequestYarpMetricCollectionMiddleware>();

        // 用于拦截WebSocket连接并收集暴露给WebSocketsTemetryConsumer的遥测的中间件
        app.UseWebSocketsTelemetry();
    }
}