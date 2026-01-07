using Masa.BuildingBlocks.Service.Caller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace LzqNet.Caller.Common;
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpContext? _httpContext;

    public ApiAuthenticationService(IServiceProvider serviceProvider)
    {
        _httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    }

    public async Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        if (_httpContext == null || HasAllowAnonymousAttribute())
        {
            return; // 不设置Authorization头
        }

        if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader) &&
            !string.IsNullOrEmpty(authHeader))
        {
            requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        }
    }

    private bool HasAllowAnonymousAttribute()
    {
        var endpoint = _httpContext?.GetEndpoint();
        return endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
    }
}