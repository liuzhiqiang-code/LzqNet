namespace LzqNet.Extensions.Serilog;


public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpLoggingMiddleware> _logger;

    public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP {Method} {Path} failed",
                context.Request.Method,
                context.Request.Path);
            throw;
        }
    }
}