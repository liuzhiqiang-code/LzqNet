using Daily.Carp.Extension;
using Daily.Carp.Feature;
using Daily.Carp.Provider.Consul;

namespace LzqNet.ApiGateway.Extensions;

public static class CarpExtensions
{
    public static void AddCustomCarp(this IHostApplicationBuilder builder)
    {
        CarpApp.Configuration = builder.Configuration;
        // 1. 添加YARP服务
        builder.Services.AddCarp().AddConsul();

        // 配置日志记录
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }
}
