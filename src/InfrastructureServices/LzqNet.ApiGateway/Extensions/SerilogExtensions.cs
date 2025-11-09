using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Diagnostics;

namespace LzqNet.ApiGateway.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// 客制化授权策略
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyName"></param>
    public static void AddCustomSerilog(this IHostApplicationBuilder builder)
    {
        // 从配置中读取日志配置
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With<ActivityTraceIdEnricher>();

        // 开发环境配置
        if (builder.Environment.IsDevelopment())
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [TraceId:{TraceId}] {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            // 生产环境配置
            loggerConfig.WriteTo.Console(new CompactJsonFormatter());

            // 可添加文件日志
            loggerConfig.WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [TraceId:{TraceId}] {Message:lj}{NewLine}{Exception}");
        }

        // 创建Logger并注册到DI容器
        Log.Logger = loggerConfig.CreateLogger();
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        });
    }
}

// 自定义TraceId增强器
public class ActivityTraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        var property = propertyFactory.CreateProperty("TraceId", traceId);
        logEvent.AddPropertyIfAbsent(property);
    }
}