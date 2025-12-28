using LzqNet.Extensions.Auth;
using LzqNet.Extensions.HealthCheck;
using LzqNet.Extensions.JsonOptions;
using LzqNet.Extensions.OAuth;
using LzqNet.Extensions.Serilog;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Text.Json.Serialization;

namespace LzqNet.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        Log.Information("Start AddApplicationServices");

        builder.Services.AddAutoInject();

        builder.AddCustomHealthChecks();
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        builder.AddCustomAuthentication();
        builder.AddCustomAuthorization();

        // 前后端类型转换  处理日期及long精度丢失问题
        // 1. 配置 JSON 选项，将所有 long 序列化为 string
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new LongToStringConverter());
            options.SerializerOptions.Converters.Add(new LongNullableToStringConverter());
        });

        builder.AddCustomMasa();
    }

    public static void MapApplicationServices(this WebApplication app)
    {
        Log.Information("Start MapApplicationServices");

        //app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseCustomMasaExceptionHandler();

        app.UseMiddleware<HttpLoggingMiddleware>();

        app.UseCustomAuthentication();
        app.UseCustomAuthorization();
        app.MapOpenApi();
        app.MapCustomHealthChecks();
        app.MapConfigurationApi();
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
