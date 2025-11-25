using LzqNet.Contracts.Notify.QQMail.Commands;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net.Mail;

namespace LzqNet.Services.Notify.Services;

public class MailService : ServiceBase
{
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    /// <summary>
    /// QQ邮箱 发送邮件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("QQ邮箱 发送邮件")]
    public async Task<IResult> QQMailSend(HttpRequest request)
    {
        var form = await request.ReadFormAsync();
        await EventBus.PublishAsync(new QQMailSendCommand
        {
            Subject = form["Subject"].ToString(),
            Content = form["Content"].ToString(),
            IsHtml = bool.TryParse(form["IsHtml"], out var isHtmlParsed) && isHtmlParsed,
            Tos = form["Tos"].ToString().Split(',').ToList(),
            Ccs = !string.IsNullOrEmpty(form["Ccs"])
                ? form["Ccs"].ToString().Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList()
                : null,
            Attachments = form.Files
        });
        return Results.Ok();
    }
}
