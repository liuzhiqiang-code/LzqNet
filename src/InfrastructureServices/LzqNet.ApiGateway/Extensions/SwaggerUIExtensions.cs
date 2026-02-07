using LzqNet.Caller.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using NSwag.AspNetCore;
using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LzqNet.ApiGateway.Extensions;

public static class SwaggerUIExtensions
{
    public static void AddCustomSwaggerUI(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomSwaggerUI");
        builder.Services.AddOpenApiDocument(document =>
        {
            document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
            {
                Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                Description = "请输入 Token，格式为: Bearer {your_token}"
            });

            // 让所有接口默认要求认证（或者根据需要配置）
            document.OperationProcessors.Add(
                new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        //var swaggerOptions = builder.Configuration.GetSection("Swagger").Get<SwaggerOption>() ??
        //    throw new InvalidOperationException($"未找到配置项:Swagger");
        //// 添加Swagger服务
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen(c =>
        //{
        //    foreach (var doc in swaggerOptions.Endpoints)
        //    {
        //        c.SwaggerDoc(doc.Key, new OpenApiInfo { Title = doc.Title, Version = doc.Version });
        //    }

        //    c.DocInclusionPredicate((docName, description) => true);
        //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        //    {
        //        Description = "请输入正确的Token格式： Bearer xxx",
        //        Name = "Authorization",
        //        In = ParameterLocation.Header,
        //        Type = SecuritySchemeType.ApiKey,
        //        BearerFormat = "JWT",
        //        Scheme = "Bearer"
        //    });
        //});
        //// 添加Swagger的CORS支持
        //builder.Services.ConfigureSwaggerGen(c =>
        //{
        //    c.CustomSchemaIds(type => type.FullName);
        //});
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


        // 3. 配置 NSwag UI 聚合
        //app.UseSwaggerUi();
        app.UseSwaggerUi(settings =>
        {
            // 配置不同的微服务入口
            // 这里的地址必须匹配上面 YARP 定义的 Route Path
            foreach (var doc in swaggerOptions.Endpoints)
            {
                settings.SwaggerRoutes.Add(new SwaggerUiRoute(doc.Title, $"/swagger-proxy?url={doc.Url}&key={doc.Key}"));
            }

            // UI 自身的路径
            settings.Path = "/swagger";

            // 【关键】配置持久化 Token，刷新页面后不需要重新输入
            settings.PersistAuthorization = true;
        });
    }
}
