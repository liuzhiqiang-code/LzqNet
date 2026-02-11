using LzqNet.Extensions.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace LzqNet.Extensions.Auth;

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

        builder.Services.AddOptions<JwtOption>().BindConfiguration("Jwt")
            .Validate(setting =>
                !string.IsNullOrWhiteSpace(setting.Audience) &&
                !string.IsNullOrWhiteSpace(setting.Authority),
                $"未找到配置项:Jwt");

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

            // 配置Token验证参数
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 验证发行者
                ValidateIssuer = true,
                ValidIssuer = jwtOption.Issuer,

                // 验证订阅者
                ValidateAudience = true,
                ValidAudience = jwtOption.Audience,

                // 验证Token有效期
                ValidateLifetime = true,

                // 验证签名密钥
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(AesEncryption.AesEncrypt(jwtOption.Secret, "jwt"))),

                // 允许的时钟偏差（避免时间不同步问题）
                ClockSkew = TimeSpan.FromMinutes(5),

                // 是否需要过期时间
                RequireExpirationTime = true,
            };

            // 事件处理
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Error($"认证失败: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void UseCustomAuthentication(this WebApplication app)
    {
        Log.Information("Start UseCustomAuthentication");
        app.UseAuthentication();
    }
}
