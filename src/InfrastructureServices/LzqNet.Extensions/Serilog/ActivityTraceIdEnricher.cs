using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace LzqNet.Extensions.Serilog;

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