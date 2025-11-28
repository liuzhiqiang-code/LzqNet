using LzqNet.Caller.Notify.Contracts.QQMail.Commands;
using Masa.BuildingBlocks.Dispatcher.Events;
using System.ComponentModel;

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
    public async Task<AdminResult> QQMailSend(HttpRequest request)
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
        return AdminResult.Success("发送成功");
    }
}
