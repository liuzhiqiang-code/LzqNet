using Serilog.Core;
using Serilog.Events;

namespace LzqNet.Extensions.Serilog;


public class HttpRequestEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpRequestEnricher() : this(new HttpContextAccessor())
    {
    }

    public HttpRequestEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var method = propertyFactory.CreateProperty("Method", httpContext.Request.Method);
        var path = propertyFactory.CreateProperty("Path", httpContext.Request.Path);

        logEvent.AddPropertyIfAbsent(method);
        logEvent.AddPropertyIfAbsent(path);
    }
}