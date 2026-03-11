using LzqNet.Common.Options;
using LzqNet.Extensions.AI;
using LzqNet.Extensions.HealthCheck;
using LzqNet.Extensions.Jwt;
using LzqNet.Extensions.NSwag;
using LzqNet.Extensions.Serilog;
using LzqNet.Extensions.SignalR;
using LzqNet.Extensions.SignalR.Models;
using LzqNet.Extensions.SqlSugar;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace LzqNet.Services.Msm.Extensions;

public static class ProgramExtensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        Log.Information("Start AddApplicationServices");

        builder.Services.AddOptions<GlobalConfig>().BindConfiguration("GlobalConfig");

        builder.AddLzqHealthChecks();
        builder.AddLzqNSwag();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        builder.AddLzqJwt();

        // 前后端类型转换  处理日期及long精度丢失问题
        // 1. 配置 JSON 选项，将所有 long 序列化为 string
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new LongToStringConverter());
            options.SerializerOptions.Converters.Add(new LongNullableToStringConverter());
        });

        builder.AddLzqAI();
        builder.AddLzqMasaAssembly();
        builder.AddLzqMasaSnowflake();
        builder.AddLzqSqlSugar();

        builder.AddLzqSignalRRedis(options =>
        {
            var newoptions = builder.Configuration.GetRequiredSection("SignalRRedisSettings").Get<SignalRRedisSettings>();
            options.url = newoptions.url;
            options.password = newoptions.password;
            options.port = newoptions.port;
            options.defaultDatabase = newoptions.defaultDatabase;
            options.configurationChannel = newoptions.configurationChannel;
            options.hostname = newoptions.hostname;
            options.cacheMySignalRKeyName = newoptions.cacheMySignalRKeyName;
        });

        builder.AddLzqMasa();
    }

    public static void MapApplicationServices(this WebApplication app)
    {
        Log.Information("Start MapApplicationServices");

        //app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseLzqMasaExceptionHandler();

        app.UseMiddleware<HttpLoggingMiddleware>();

        app.UseLzqNSwag();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapLzqHealthChecks();
        app.MapConfigurationApi();

        app.UseLzqRedisSignalR(options =>
        {
            var newoptions = app.Configuration.GetRequiredSection("SignalRRedisSettings").Get<SignalRRedisSettings>();
            options.url = newoptions.url;
            options.password = newoptions.password;
            options.port = newoptions.port;
            options.defaultDatabase = newoptions.defaultDatabase;
            options.configurationChannel = newoptions.configurationChannel;
            options.hostname = newoptions.hostname;
            options.cacheMySignalRKeyName = newoptions.cacheMySignalRKeyName;
        });

        app.MapMasaMinimalAPIs();
    }

    public static void MapConfigurationApi(this WebApplication app)
    {
        app.MapGet("/configuration", [Authorize] (HttpContext context, IConfiguration configuration, string key) =>
        {
            var section = configuration.GetSection(key);

            if (!section.Exists())
            {
                return Results.NotFound($"配置节点 {key} 不存在");
            }

            // 递归构建配置树，确保所有子节点格式正确
            object BuildConfigTree(IConfigurationSection currentSection)
            {
                if (currentSection.Value != null)
                {
                    return currentSection.Value; // 叶子节点直接返回值
                }

                var children = currentSection.GetChildren().ToList();
                if (!children.Any())
                {
                    return null; // 空节点返回 null
                }

                // 处理数组格式（如 Endpoints:0:Key）
                if (children.All(c => int.TryParse(c.Key, out _)))
                {
                    return children.Select(BuildConfigTree).ToList();
                }

                // 处理对象格式
                var dict = new Dictionary<string, object>();
                foreach (var child in children)
                {
                    var childValue = BuildConfigTree(child);
                    if (childValue != null)
                    {
                        dict[child.Key] = childValue;
                    }
                }
                return dict;
            }

            var configTree = BuildConfigTree(section);
            return Results.Ok(new Dictionary<string, object> { [key] = configTree });
        });
    }
}
