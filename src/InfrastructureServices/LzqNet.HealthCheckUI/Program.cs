//using HealthChecks.UI.Client;
//using HealthChecks.UI.Core;
//using LzqNet.DCC;
//using LzqNet.HealthCheckUI.Services;
//using Microsoft.AspNetCore.Diagnostics.HealthChecks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Diagnostics.HealthChecks;

//var builder = WebApplication.CreateBuilder(args);

//builder.AddApplicationConfiguration();

//// 添加健康检查服务
//builder.Services.AddHealthChecks()
//    // 添加当前监控服务本身的健康检查
//    .AddCheck("healthcheck-ui", () => HealthCheckResult.Healthy("HealthCheck UI 服务运行正常"));

//// 配置 HealthChecks UI
//builder.Services.AddHealthChecksUI(setup =>
//{
//    // 从配置文件中读取要监控的端点
//    var healthChecksUIOption = builder.Configuration.GetSection("HealthChecksUI")
//        .Get<HealthChecksUIOption>();
//    if (healthChecksUIOption == null)
//        throw new InvalidOperationException($"未找到配置项:HealthChecksUI");

//    if (healthChecksUIOption.HealthChecks != null)
//    {
//        foreach (var endpoint in healthChecksUIOption.HealthChecks)
//            setup.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);
//    }

//    setup.SetEvaluationTimeInSeconds(healthChecksUIOption.EvaluationTimeInSeconds); // 每60秒检查一次
//    setup.SetMinimumSecondsBetweenFailureNotifications(healthChecksUIOption.MinimumSecondsBetweenFailureNotifications); // 检查失败最小间隔时间
//    setup.SetApiMaxActiveRequests(healthChecksUIOption.ApiMaxActiveRequests); // 最大并发请求数
//    setup.MaximumHistoryEntriesPerEndpoint(healthChecksUIOption.MaximumHistoryEntriesPerEndpoint); // 每个端点保存50条历史记录

//    // 配置 Webhook 通知
//    var webhooks = healthChecksUIOption.Webhooks;

//    if (webhooks != null)
//    {
//        foreach (var webhook in webhooks)
//        {
//            setup.AddWebhookNotification(
//                webhook.Name,
//                webhook.Uri,
//                webhook.Payload,
//                webhook.RestorePayload,
//                shouldNotifyFunc: (string sad, UIHealthReport report) => { 
//                    return true;
//                },
//                customMessageFunc: (string sad, UIHealthReport report) => {
//                    return "customMessageFunc";
//                },
//                customDescriptionFunc: (string sad, UIHealthReport report) => {
//                    return "customDescriptionFunc";
//                }


//            );
//        }
//    }
//})
//.AddInMemoryStorage(); // 使用内存存储

//builder.Services.AddMasaMinimalAPIs();

//var app = builder.Build();

//app.MapMasaMinimalAPIs();

//// 配置健康检查端点
//app.MapHealthChecks("/health", new HealthCheckOptions
//{
//    Predicate = _ => true,
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});

//// 配置 HealthChecks UI 仪表板
//app.MapHealthChecksUI(options =>
//{
//    options.UIPath = "/health-ui"; // UI 访问路径
//    options.ApiPath = "/health-ui-api"; // API 路径
//});

//// 重定向根路径到健康检查 UI
//app.MapGet("/", () => Results.Redirect("/health-ui"));

//app.MapGet("/send", ([FromBody] WebhookSendDto input) => {
//    var das = input.ToJson();
//    return Results.Ok(das);
//});

//app.Run();


using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json; // 引入 System.Text.Json
using System.Collections.Generic;
using System.Linq;



// --- 2. 应用程序初始化 (Application Initialization) ---

var builder = WebApplication.CreateBuilder(args);

// --- 3. 模拟配置数据实例化 (Simulate Configuration Data Instantiation) ---
var healthChecksUIOption = new HealthChecksUIOption
{
    EvaluationTimeInSeconds = 5, // 5秒检查一次
    MinimumSecondsBetweenFailureNotifications = 60,
    HealthChecks = new List<HealthCheckEndpoint>
    {
        // 监控当前服务自身的健康检查端点 (Healthy)
        new HealthCheckEndpoint { Name = "Local UI Self Check", Uri = "/health" },
        // 监控一个永远失败的模拟外部服务 (Unhealthy)，用于触发 Webhook
        new HealthCheckEndpoint { Name = "Failing Mock Service", Uri = "/fail-health" }
    },
    Webhooks = new List<WebhookNotification>
    {
        new WebhookNotification
        {
            Name = "Custom Notification API",
            // 【关键修改】使用相对路径 /send，更可靠地触发本地 Webhook
            Uri = "/send", 
            // HealthChecks.UI 默认发送的 JSON Payload 模板
            Payload = "{\"ServiceName\":\"{{serviceName}}\",\"Status\":\"{{status}}\",\"Message\":\"Service {{serviceName}} is {{status}}\"}",
            RestorePayload = "{\"ServiceName\":\"{{serviceName}}\",\"Status\":\"{{status}}\",\"Message\":\"Service {{serviceName}} is back to {{status}}\"}"
        }
    }
};

// 4. 添加 Health Checks 服务
builder.Services.AddHealthChecks()
    // 添加当前监控服务本身的健康检查 (Healthy)
    .AddCheck("ui_self_check", () => HealthCheckResult.Healthy("HealthCheck UI 服务运行正常"))
    // 添加一个永远失败的检查 (Unhealthy)，用于演示 Webhook 触发
    .AddCheck("always_failing_check", () => HealthCheckResult.Unhealthy("模拟外部服务检查失败"));

// 5. 配置 HealthChecks UI
builder.Services.AddHealthChecksUI(setup =>
{
    // 从模拟配置中读取要监控的端点
    foreach (var endpoint in healthChecksUIOption.HealthChecks)
        setup.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);

    setup.SetEvaluationTimeInSeconds(healthChecksUIOption.EvaluationTimeInSeconds);
    setup.SetMinimumSecondsBetweenFailureNotifications(healthChecksUIOption.MinimumSecondsBetweenFailureNotifications);
    setup.MaximumHistoryEntriesPerEndpoint(healthChecksUIOption.MaximumHistoryEntriesPerEndpoint);

    // 配置 Webhook 通知
    foreach (var webhook in healthChecksUIOption.Webhooks)
    {
        setup.AddWebhookNotification(
            webhook.Name,
            webhook.Uri,
            webhook.Payload,
            webhook.RestorePayload,
            // 【自定义 Should Notify 逻辑】仅在出现不健康状态时发送通知
            shouldNotifyFunc: (string notificationName, UIHealthReport report) =>
            {
                // 如果发现有任何不健康的端点，则发送通知
                return report.Status != UIHealthStatus.Healthy;
            },
            // 【自定义消息函数】覆盖默认消息
            customMessageFunc: (string notificationName, UIHealthReport report) =>
            {
                var unhealthyCount = report.Entries.Count(e => e.Value.Status != UIHealthStatus.Healthy);
                return $"【自定义通知】系统报告：发现 {unhealthyCount} 个不健康的端点。";
            },
            // 【自定义描述函数】覆盖默认描述
            customDescriptionFunc: (string notificationName, UIHealthReport report) =>
            {
                var failingServices = report.Entries
                    .Where(e => e.Value.Status != UIHealthStatus.Healthy)
                    .Select(e => $"{e.Key} ({e.Value.Status})")
                    .ToList();
                return $"【不健康列表】: {string.Join(", ", failingServices)}";
            }
        );
    }
})
.AddInMemoryStorage(); // 使用内存存储

var app = builder.Build();

// --- 6. 配置端点 (Configure Endpoints) ---

// 6a. 配置 Health Checks 端点 (供 UI 监控自身状态)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("ui_self_check"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// 6b. 模拟一个外部服务的健康检查端点 (永远失败，供 UI 监控)
app.MapHealthChecks("/fail-health", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("failing_check"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// 6c. 配置 HealthChecks UI 仪表板
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui"; // UI 访问路径
    options.ApiPath = "/health-ui-api"; // API 路径
});

// 6d. 自定义 Webhook API 接收端点 (实现用户要求的 Custom API)
// HealthChecks UI 的 Webhook 会 POST 数据到这个端点
app.MapPost("/send", async ([FromBody] JsonElement rawInput) =>
{
    // 将接收到的原始 JSON 对象格式化输出到控制台
    var jsonString = JsonSerializer.Serialize(rawInput, new JsonSerializerOptions { WriteIndented = true });

    Console.WriteLine($"\n--- 接收到来自 HealthChecks UI 的 Webhook 通知 (Custom API) ---");
    Console.WriteLine("这是您的自定义API，您可以在这里集成第三方通知服务 (如钉钉/企业微信/Slack等)。");
    Console.WriteLine(jsonString);
    Console.WriteLine("-------------------------------------------------------------\n");

    // 返回成功响应给 HealthChecks UI
    return Results.Ok(new { message = "Webhook 通知已成功接收和处理。", receivedData = jsonString });
})
.Accepts<JsonElement>("application/json") // 确保它接受 JSON
.Produces(StatusCodes.Status200OK);


// 6e. 重定向根路径到健康检查 UI
app.MapGet("/", () => Results.Redirect("/health-ui"));

app.Run();


// --- 1. 模拟配置和 DTO 类定义 (Simulate Configuration and DTO Class Definitions) ---

/// <summary>
/// 模拟配置中的健康检查端点
/// </summary>
public class HealthCheckEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

/// <summary>
/// 模拟配置中的 Webhook 通知设置
/// </summary>
public class WebhookNotification
{
    public string Name { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string RestorePayload { get; set; } = string.Empty;
}

/// <summary>
/// 模拟 HealthChecks UI 配置选项
/// </summary>
public class HealthChecksUIOption
{
    public int EvaluationTimeInSeconds { get; set; } = 5; // 5秒检查一次，便于演示
    public int MinimumSecondsBetweenFailureNotifications { get; set; } = 60;
    public int ApiMaxActiveRequests { get; set; } = 1;
    public int MaximumHistoryEntriesPerEndpoint { get; set; } = 50;
    public List<HealthCheckEndpoint> HealthChecks { get; set; } = new();
    public List<WebhookNotification> Webhooks { get; set; } = new();
}

/// <summary>
/// Webhook 接收端点的数据传输对象 (DTO)
/// 注意：实际 HealthChecks UI 发送的 Payload 是一个字符串模板替换后的 JSON，
/// 此处定义是为了演示目的，在 MapPost("/send") 中，我们直接接收原始 JSON 对象。
/// </summary>
public class WebhookSendDto
{
    public string ServiceName { get; set; } = "Unknown Service";
    public string Status { get; set; } = "Unknown Status";
    public string Message { get; set; } = "No Message";
}