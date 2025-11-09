using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Health;
using Yarp.ReverseProxy.Model;

namespace LzqNet.ApiGateway;

public class CustomPassiveHealthCheckPolicy : IPassiveHealthCheckPolicy
{
    private readonly ILogger<CustomPassiveHealthCheckPolicy> _logger;
    private readonly IDestinationHealthUpdater _healthUpdater;
    private readonly TimeSpan _defaultReactivationPeriod = TimeSpan.FromMinutes(5); // 默认恢复时间5分钟

    public string Name => "CustomPassiveHealthCheckPolicy";

    public CustomPassiveHealthCheckPolicy(IDestinationHealthUpdater healthUpdater, ILogger<CustomPassiveHealthCheckPolicy> logger)
    {
        _healthUpdater = healthUpdater;
        _logger = logger;
    }

    public void RequestProxied(HttpContext context, ClusterState cluster, DestinationState destination)
    {
        // 请求成功代理时的处理
        var statusCode = context.Response.StatusCode;

        // 根据HTTP状态码判断健康状态
        if (statusCode >= 500)
        { 
            // 从集群配置获取恢复时间，若无则使用默认值
            var reactivationPeriod = cluster.Model.Config.HealthCheck?.Passive?.ReactivationPeriod
                ?? _defaultReactivationPeriod;
            // 服务器错误，标记为不健康
            _healthUpdater.SetPassive(
               cluster,
               destination,
               DestinationHealth.Unhealthy,
               reactivationPeriod
           );
            _logger.LogWarning($"Destination {destination.DestinationId} marked unhealthy due to status code {statusCode}");
        }
        else if (statusCode >= 400 && statusCode < 500)
        {
            // 客户端错误，通常不标记为不健康（可能是请求问题）
            // 可以根据需要调整逻辑
            destination.Health.Passive = DestinationHealth.Healthy;
            // _healthUpdater.SetPassive(
            //    cluster,
            //    destination,
            //    DestinationHealth.Unhealthy,
            //    TimeSpan.FromMinutes(1)
            //);
            _logger.LogWarning($"Destination {destination.DestinationId} marked unhealthy due to status code {statusCode}");
        }
        else
        {
            // 成功响应，标记为健康
            destination.Health.Passive = DestinationHealth.Healthy;
        }
    }
}
