using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LzqNet.ApiGateway.Metrics;

/// <summary>
///  此中间件记录每个请求处理各阶段耗时（路由、代理、HTTP通信等）的中间件
///  与Prometheus的聚合指标不同，此中间件侧重于每个请求的详细指标收
///  直接集成到日志系统中
/// </summary>
public class PerRequestYarpMetricCollectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerRequestYarpMetricCollectionMiddleware> _logger;

    public PerRequestYarpMetricCollectionMiddleware(RequestDelegate next, ILogger<PerRequestYarpMetricCollectionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var metrics = PerRequestMetrics.Current;
        metrics.StartTime = DateTime.UtcNow;

        await _next(context);

        // 在其他中间件步骤完成后调用
        // 通过ILogger将信息写入控制台。在你可能想要的生产场景中将结果直接写入遥测系统
        _logger.LogInformation("PerRequestMetrics: "+ metrics.ToJson());
    }
}