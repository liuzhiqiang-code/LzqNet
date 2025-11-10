using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace LzqNet.ApiGateway.Extensions;

public static class RateLimiterExtensions
{
    public static void AddCustomRateLimiter(this IHostApplicationBuilder builder, string policyName)
    {
        var rateLimiterOptions = builder.Configuration.GetSection("RateLimiter").Get<List<RateLimiterOption>>();
        if (rateLimiterOptions == null)
            throw new InvalidOperationException("未找到配置项:RateLimiter");
        var rateLimiterOption = rateLimiterOptions.FirstOrDefault(x => x.PolicyName.ToString() == policyName);
        if (rateLimiterOption == null)
            throw new InvalidOperationException($"未找到配置项:RateLimiter->policyName={policyName}");
        builder.Services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter(policyName: rateLimiterOption!.PolicyName, options =>
            {
                //  2s内只允许 1 次请求，超过则拒绝
                options.PermitLimit = rateLimiterOption.PermitLimit;  //每个时间窗口内允许的最大请求数（此处为 1 次请求）。
                options.Window = TimeSpan.FromSeconds(rateLimiterOption.WindowInSeconds);  //时间窗口长度（此处为 2 秒）。
                if (rateLimiterOption.QueueProcessingOrder != null)
                    options.QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(
                        rateLimiterOption.QueueProcessingOrder,
                        ignoreCase: true  // 可选：忽略大小写
                    );
                if (rateLimiterOption.QueueLimit.HasValue)
                    options.QueueLimit = rateLimiterOption.QueueLimit.Value;  //队列最大长度，超过则拒绝请求
            }));
    }
}
