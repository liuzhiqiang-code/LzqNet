using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace LzqNet.SwaggerUI.Extensions.HealthCheck;
public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var memoryUsage = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
        if (memoryUsage > 1024) // 超过1GB
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"内存使用率过高: {memoryUsage}MB"));
        }
        return Task.FromResult(HealthCheckResult.Healthy(
            $"内存使用率正常: {memoryUsage}MB"));
    }
}