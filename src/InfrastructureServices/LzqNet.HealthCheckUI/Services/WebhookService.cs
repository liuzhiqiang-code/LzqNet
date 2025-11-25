using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using static IdentityModel.OidcConstants;

namespace LzqNet.HealthCheckUI.Services;

public class WebhookService : ServiceBase
{
    /// <summary>
    /// 健康检查 发送通知 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("健康检查 发送通知")]
    [HttpPost]
    public async Task<IResult> Send([FromBody] WebhookSendDto input)
    {
        var das = input.ToJson();
        return Results.Ok(das);
    }
}
public record WebhookSendDto
{
    string Type { get; set; }
    string Service { get; set; }
    string Failure { get; set; }
    string Description { get; set; }
    string Timestamp { get; set; }
}