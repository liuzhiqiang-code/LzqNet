namespace LzqNet.ApiGateway.Extensions;

public static class HealthChecksUIExtensions
{
    public static void AddCustomHealthChecksUI(this IHostApplicationBuilder builder)
    {
        //Log.Information("Start AddCustomHealthChecksUI");

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
        //})
        //.AddInMemoryStorage(); // 使用内存存储
    }

    public static void MapCustomHealthChecksUI(this WebApplication app)
    {
        //// 配置 HealthChecks UI 仪表板
        //app.MapHealthChecksUI(options =>
        //{
        //    options.UIPath = "/health-ui"; // UI 访问路径
        //    options.ApiPath = "/health-ui-api"; // API 路径
        //});
    }
}
