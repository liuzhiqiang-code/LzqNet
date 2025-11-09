using Microsoft.AspNetCore.Authorization;
using Serilog;
using Serilog.Core;

namespace LzqNet.ApiGateway.Extensions;

public static class AuthorizationExtensions
{
    /// <summary>
    /// 客制化授权策略
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyName"></param>
    public static void AddCustomAuthorization(this IHostApplicationBuilder builder, string policyName = "default")
    {
        Log.Information("Start AddCustomAuthorization");

        // 网关一般的授权策略是只过滤掉不合法身份认证的请求，更详细的授权在服务中认证
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(policyName, policy => policy.RequireAuthenticatedUser());

            //退回策略   ReverseProxy=>Routes=>AuthorizationPolicy  指定使用的授权策略,没指定使用退回策略
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser() // 默认情况下，要求用户已通过身份验证. 可以提成你需要的验证
                .Build();
        });
    }

    public static void UseCustomAuthorization(this WebApplication app)
    {
        Log.Information("Start UseCustomAuthorization");
        app.UseAuthorization();
    }
}