using Microsoft.AspNetCore.Authorization;

namespace LzqNet.ApiGateway.Extensions;

public static class ResponseCachingExtensions
{
    /// <summary>
    /// 客制化授权策略
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyName"></param>
    public static void AddCustomResponseCaching(this IHostApplicationBuilder builder)
    {
        builder.Services.AddResponseCaching(options =>
        {
            options.UseCaseSensitivePaths = false; //确定是否将响应缓存在区分大小写的路径上。 
            options.SizeLimit = options.SizeLimit * 10; // 响应缓存中间件的大小限制（以字节为单位） 1G
        });
    }

    public static void UseCustomResponseCaching(this WebApplication app)
    {
        // 拦截请求并判断 请求头中是否包含 CacheControl 标头，如果没有则加上缓存标头
        app.Use(async (context, next) =>
        {
            var header = context.Request.Headers;
            var cacheControl = header.CacheControl;
            if (!string.IsNullOrEmpty(header.CacheControl))
            {
                header.CacheControl = new Microsoft.Extensions.Primitives.StringValues("max-age");
            }

            await next(context);
        });
        app.UseResponseCaching();
        app.Use(async (context, next) =>
        {
            context.Response.GetTypedHeaders().CacheControl =
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(3)
                };

            context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

            await next(context);
        });
    }
}
