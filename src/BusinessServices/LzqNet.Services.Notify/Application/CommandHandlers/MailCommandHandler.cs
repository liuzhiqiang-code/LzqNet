using LzqNet.Contracts.Notify.QQMail.Commands;
using LzqNet.Services.Notify.Domain.Repositories;
using Masa.Contrib.Dispatcher.Events;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LzqNet.Services.Notify.Application.CommandHandlers;

public class MailCommandHandler(ISysConfigRepository sysConfigRepository,
    ILogger<MailCommandHandler> logger)
{
    private readonly ISysConfigRepository _sysConfigRepository = sysConfigRepository;
    private readonly ILogger<MailCommandHandler> _logger = logger;

    [EventHandler]
    public async Task QQMailSendHandleAsync(QQMailSendCommand command)
    {
        // 1. 获取QQ邮箱SMTP配置
        var smtpConfig = new {
            Host = "smtp.qq.com",
            Port = 587,
            Account = "1324445692@qq.com",
            Password = "buexlfodonjbibei",
            Address = "1324445692@qq.com"
        };
        try
        {
            using var smtp = new SmtpClient(smtpConfig.Host, smtpConfig.Port);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(smtpConfig.Account, smtpConfig.Password);
            smtp.EnableSsl = true; // 启用加密
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            using var mail = new MailMessage();
            mail.From = new MailAddress(smtpConfig.Address);

            foreach (var to in command.Tos)
                mail.To.Add(to.Trim());//接收人

            if (command.Ccs != null && command.Ccs.Count > 0)
                foreach (var cc in command.Ccs)
                    mail.CC.Add(cc.Trim());//接收人

            mail.SubjectEncoding = Encoding.UTF8;
            mail.Subject = command.Subject;//邮件主题
            mail.IsBodyHtml = command.IsHtml;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Body = command.Content;//邮件内容

            if (command.Attachments != null && command.Attachments.Count > 0)
                foreach (var attachment in command.Attachments)
                    mail.Attachments.Add(
                        new Attachment(attachment.OpenReadStream(),
                        attachment.FileName));//附件内容

            await smtp.SendMailAsync(mail); // 异步发送邮件
        }
        catch (Exception ex)
        {
            // 记录详细日志（需要注入 ILogger）
            _logger.LogError(ex, "QQ邮件发送失败。收件人: {To}, 主题: {Subject}", command.Tos.ToJson(), command.Subject);
            throw;
        }
    }
}
