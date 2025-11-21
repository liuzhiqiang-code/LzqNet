using LzqNet.Contracts.Notify.QQMail.Commands;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LzqNet.Services.Notify.Services;

public class MailService : ServiceBase
{
    private IEventBus EventBus => GetRequiredService<IEventBus>();

    ///// <summary>
    ///// QQ邮箱 发送邮件 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[DisplayName("QQ邮箱 发送邮件")]
    //public async Task<IResult> QQMailSend([FromForm]QQMailSendCommand input)
    //{
    //    await EventBus.PublishAsync(input);
    //    return Results.Ok();
    //}
}
