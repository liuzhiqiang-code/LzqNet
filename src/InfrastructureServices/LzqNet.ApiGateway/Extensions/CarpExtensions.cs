using Daily.Carp;
using Daily.Carp.Extension;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Health;

namespace LzqNet.ApiGateway.Extensions;

public static class CarpExtensions
{
    public static void AddCustomCarp(this IHostApplicationBuilder builder)
    {
        CarpApp.Configuration = builder.Configuration;
        // 1. 添加YARP服务
        builder.Services.AddCarp().AddConsul();
    }
}
