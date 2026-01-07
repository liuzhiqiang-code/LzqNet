public class HealthChecksUIOption
{
    public List<HealthCheckSettings> HealthChecks { get; set; } = new();

    /// <summary>
    /// 每60秒检查一次
    /// </summary>
    public int EvaluationTimeInSeconds { get; set; } = 60;

    /// <summary>
    /// 检查失败最小间隔时间
    /// </summary>
    public int MinimumSecondsBetweenFailureNotifications { get; set; } = 60;

    /// <summary>
    /// 最大并发请求数
    /// </summary>
    public int ApiMaxActiveRequests { get; set; } = 3;

    /// <summary>
    /// 每个端点保存50条历史记录
    /// </summary>
    public int MaximumHistoryEntriesPerEndpoint { get; set; } = 50;
}


// 健康检查设置类
public class HealthCheckSettings
{
    public string Name { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}