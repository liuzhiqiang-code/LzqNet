using LzqNet.Extensions.SignalR.Models;
using LzqNet.Extensions.SignalR.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace LzqNet.Extensions.SignalR;

public static class SignalRExtensions
{
    public static void AddLzqSignalRRedis(this WebApplicationBuilder builder, Action<SignalRRedisSettings> action)
    {
        var services = builder.Services;
        services.Configure(action);
        var setting = new SignalRRedisSettings();
        action.Invoke(setting);
        services.AddSignalR(options =>
        {
            //客户端发保持连接请求到服务端最长间隔，默认30秒，改成4分钟
            options.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
            //服务端发保持连接请求到客户端间隔，默认15秒，改成2分钟
            options.KeepAliveInterval = TimeSpan.FromMinutes(2);
        }).AddStackExchangeRedis(op =>
        {
            op.Configuration.ConfigurationChannel = setting.configurationChannel;//Redis配置频道名称
            op.Configuration.DefaultDatabase = setting.defaultDatabase;//Redis数据库索引，默认为0
            op.Configuration.AbortOnConnectFail = false;//连接失败是否抛出异常，默认为true，改成false
            op.Configuration.Password = setting.password;
            op.Configuration.EndPoints.Add(setting.hostname, int.Parse(setting.port));//Redis服务器地址和端口
            op.Configuration.ConnectRetry = 3;//连接失败重试次数
            op.Configuration.SyncTimeout = 3000;//同步操作超时时间，单位毫秒
        });
        services.TryAddScoped<ISignalRMsgService, SignalRMsgService>();
    }

    public static void UseLzqRedisSignalR(this IApplicationBuilder app, Action<SignalRRedisSettings> action)
    {
        var setting = new SignalRRedisSettings();
        action.Invoke(setting);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<MySignalRHub>(setting.url);
        });
    }
}