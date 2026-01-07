using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LzqNet.ApiGateway.Extensions;

public static class SwaggerUIExtensions
{
    public static void AddCustomSwaggerUI(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomSwaggerUI");

        var swaggerOptions = builder.Configuration.GetSection("Swagger").Get<SwaggerOption>() ??
            throw new InvalidOperationException($"未找到配置项:Swagger");
        // 添加Swagger服务
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            foreach (var doc in swaggerOptions.Endpoints)
            {
                c.SwaggerDoc(doc.Key, new OpenApiInfo { Title = doc.Title, Version = doc.Version });
            }
        });
        // 添加Swagger的CORS支持
        builder.Services.ConfigureSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);
        });
    }

    public static void MapCustomSwaggerUI(this WebApplication app) 
    {
        var swaggerOptions = app.Configuration.GetSection("Swagger").Get<SwaggerOption>() ??
            throw new InvalidOperationException($"未找到配置项:Swagger");
        // 在 app.Run() 前添加代理中间件
        app.MapGet("/swagger-proxy", async ([FromQuery(Name = "url")] string url,
            [FromQuery(Name = "key")] string key,
            HttpClient httpClient, AuthCaller authCaller) =>
        {
            if (string.IsNullOrWhiteSpace(url))
                return Results.BadRequest("缺少参数 'url'");
            try
            {
                JwtClientOption? jwtClientOption =
                    app.Configuration.GetSection("JwtClient").Get<JwtClientOption>();
                if (jwtClientOption != null)
                {
                    var loginInfo = await authCaller.GetClientToken(jwtClientOption);
                    if (loginInfo != null)
                    {
                        // 2. 将Token添加到请求头
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", loginInfo.AccessToken);
                    }
                }
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = JsonSerializer.SerializeToNode(doc.RootElement); // 转为可修改对象

                // 仅修改servers数组中的url
                var servers = root["servers"]?.AsArray() ?? [];
                if (servers.Count == 0)
                {
                    // 显式指定匿名类型
                    var serverNode = JsonSerializer.SerializeToNode(new { url = $"{swaggerOptions.GatewayUrl}/{key}" }, new JsonSerializerOptions());
                    // 显式指定数组类型
                    root["servers"] = JsonSerializer.SerializeToNode(new[] { serverNode }, new JsonSerializerOptions());
                }
                foreach (var server in servers)
                {
                    if (key.TrimStart("/").IsNullOrEmpty())
                        server["url"] = $"{swaggerOptions.GatewayUrl}";//网关自己的服务api
                    else
                        server["url"] = $"{swaggerOptions.GatewayUrl}/{key}";
                }
                string modifiedJson = root?.ToJsonString() ?? string.Empty;
                return Results.Content(modifiedJson, response.Content.Headers.ContentType?.ToString() ?? "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: $"代理请求失败: {ex.Message}", statusCode: 500);
            }
        });

        app.UseSwaggerUI(c =>
        {
            foreach (var doc in swaggerOptions.Endpoints)
            {
                // 将原始 URL 替换为代理路径
                //c.SwaggerEndpoint(doc.Url, doc.Title);
                c.SwaggerEndpoint($"/swagger-proxy?url={doc.Url}&key={doc.Key}", doc.Title);
            }
        });

    }
}
