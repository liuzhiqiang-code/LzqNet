using Microsoft.AspNetCore.Authorization;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace LzqNet.Extensions.Serilog;

public static class SerilogExtensions
{
    /// <summary>
    /// Serilog作为日志组件
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