using LzqNet.Caller.Identity;
using LzqNet.DCC;
using LzqNet.DCC.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();

// 添加 HttpClient 服务
builder.Services.AddHttpClient();
builder.Services.AddAutoRegistrationCaller(
    typeof(Program).Assembly,
    typeof(IdentityCaller).Assembly
    );

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

var app = builder.Build();

// 在 app.Run() 前添加代理中间件
app.MapGet("/swagger-proxy", async ([FromQuery(Name = "url")] string url,
    [FromQuery(Name = "key")] string key,
    HttpClient httpClient, IdentityCaller identityCaller) =>
{
    if (string.IsNullOrWhiteSpace(url))
        return Results.BadRequest("缺少参数 'url'");
    try
    {
        List<DefaultAccountOption> defaultAccountOptions =
            builder.Configuration.GetSection("DefaultAccounts").Get<List<DefaultAccountOption>>() ?? [];
        if (defaultAccountOptions.Count > 0)
        {
            var loginInfo = await identityCaller.Login(new UserLoginDto(defaultAccountOptions[0].UserName, defaultAccountOptions[0].Password));
            if (loginInfo != null)
            {
                // 2. 将Token添加到请求头
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", loginInfo.Token);
            }
        }
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = JsonSerializer.SerializeToNode(doc.RootElement); // 转为可修改对象

        // 仅修改servers数组中的url
        var servers = root["servers"].AsArray();
        foreach (var server in servers)
        {
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

app.Run();
