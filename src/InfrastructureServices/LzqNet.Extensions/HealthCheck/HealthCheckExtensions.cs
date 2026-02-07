using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace LzqNet.Extensions.HealthCheck;
public static class HealthCheckExtensions
{
    public static void AddCustomHealthChecks(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomHealthChecks");

        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("服务运行正常"))
            .AddCheck<MemoryHealthCheck>("内存检查");
    }

    public static void MapCustomHealthChecks(this WebApplication app)
    {
        Log.Information("Start MapCustomHealthChecks");

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        }).AllowAnonymous();
    }
}

