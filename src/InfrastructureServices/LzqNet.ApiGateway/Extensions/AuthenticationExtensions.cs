using LzqNet.DCC.Option;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;

namespace LzqNet.ApiGateway.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// 认证方式
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyName"></param>
    public static void AddCustomAuthentication(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomAuthentication");

        JwtOption jwtOption = builder.Configuration.GetSection("Jwt")
            .Get<JwtOption>() ?? throw new InvalidOperationException($"未找到配置项:Jwt");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = jwtOption.Authority;
            options.RequireHttpsMetadata = jwtOption.RequireHttpsMetadata;
            options.Audience = jwtOption.Audience;
        });
    }

    public static void UseCustomAuthentication(this WebApplication app)
    {
        Log.Information("Start UseCustomAuthentication");
        app.UseAuthentication();
    }
}
