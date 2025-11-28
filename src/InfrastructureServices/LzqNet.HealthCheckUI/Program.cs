using HealthChecks.UI.Client;
using LzqNet.DCC;
using LzqNet.HealthCheckUI.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();

// 添加健康检查服务
builder.Services.AddHealthChecks()
    // 添加当前监控服务本身的健康检查
    .AddCheck("healthcheck-ui", () => HealthCheckResult.Healthy("HealthCheck UI 服务运行正常"));

// 配置 HealthChecks UI
builder.Services.AddHealthChecksUI(setup =>
{
    // 从配置文件中读取要监控的端点
    var healthChecksUIOption = builder.Configuration.GetSection("HealthChecksUI")
        .Get<HealthChecksUIOption>();
    if (healthChecksUIOption == null)
        throw new InvalidOperationException($"未找到配置项:HealthChecksUI");

    if (healthChecksUIOption.HealthChecks != null)
    {
        foreach (var endpoint in healthChecksUIOption.HealthChecks)
            setup.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);
    }

    setup.SetEvaluationTimeInSeconds(healthChecksUIOption.EvaluationTimeInSeconds); // 每60秒检查一次
    setup.SetMinimumSecondsBetweenFailureNotifications(healthChecksUIOption.MinimumSecondsBetweenFailureNotifications); // 检查失败最小间隔时间
    setup.SetApiMaxActiveRequests(healthChecksUIOption.ApiMaxActiveRequests); // 最大并发请求数
    setup.MaximumHistoryEntriesPerEndpoint(healthChecksUIOption.MaximumHistoryEntriesPerEndpoint); // 每个端点保存50条历史记录
})
.AddInMemoryStorage(); // 使用内存存储

builder.Services.AddMasaMinimalAPIs();

var app = builder.Build();

app.MapMasaMinimalAPIs();

// 配置健康检查端点
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// 配置 HealthChecks UI 仪表板
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui"; // UI 访问路径
    options.ApiPath = "/health-ui-api"; // API 路径
});

// 重定向根路径到健康检查 UI
app.MapGet("/", () => Results.Redirect("/health-ui"));

app.MapGet("/send", ([FromBody] WebhookSendDto input) =>
{
    var das = input.ToJson();
    return Results.Ok(das);
});

app.Run();