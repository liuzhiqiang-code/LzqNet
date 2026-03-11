using LzqNet.Extensions.Jwt.Options;
using LzqNet.Extensions.Jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace LzqNet.Extensions.Jwt;

public static class JwtExtensions
{
    public static void AddLzqJwt(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddLzqJwt");

        JwtOption jwtOption = builder.Configuration.GetSection("Jwt")
            .Get<JwtOption>() ?? throw new InvalidOperationException($"未找到配置项:Jwt");

        builder.Services.AddOptions<JwtOption>().BindConfiguration("Jwt")
            .Validate(setting =>
                !string.IsNullOrWhiteSpace(setting.Audience) &&
                !string.IsNullOrWhiteSpace(setting.Authority),
                $"未找到配置项:Jwt");

        builder.Services.AddJwt(option =>
        {
            option.Issuer = jwtOption.Issuer;
            option.Audience = jwtOption.Audience;
            option.SecurityKey = jwtOption.SecurityKey;
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IJwtService, JwtService>();
        builder.Services.AddTransient<ICurrentUser, CurrentUser>();



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
                    Encoding.UTF8.GetBytes(jwtOption.SecurityKey)),

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


        // 网关一般的授权策略是只过滤掉不合法身份认证的请求，更详细的授权在服务中认证
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("default", policy => policy.RequireAuthenticatedUser());

            //退回策略   ReverseProxy=>Routes=>AuthorizationPolicy  指定使用的授权策略,没指定使用退回策略
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser() // 默认情况下，要求用户已通过身份验证. 可以提成你需要的验证
                .Build();
        });
    }
}

