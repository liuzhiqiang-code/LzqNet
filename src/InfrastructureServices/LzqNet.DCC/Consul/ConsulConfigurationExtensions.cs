using Consul.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace LzqNet.DCC.Consul;

public static class ConsulConfigurationExtensions
{
    public static void AddConsulConfiguration(this IHostApplicationBuilder builder, params List<string> configurationKeys)
    {
        var consulOptions = builder.Configuration.GetSection("Consul").Get<ConsulOptions>()
            ?? throw new InvalidOperationException($"未找到配置项:Consul");
        var consulAddress = $"{consulOptions.ConsulIP}:{consulOptions.ConsulPort}";
        if (!consulAddress.StartsWith("http://") && !consulAddress.StartsWith("https://"))
            consulAddress = "http://" + consulAddress; // 默认使用 HTTP 协议
        foreach (var configurationKey in configurationKeys)
        {
            //默认是使用Json配置文件格式，要使用其他常量格式，需要放在不同的Key里面
            builder.Configuration.AddConsul(configurationKey, options =>
            {
                options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); }; // 1、consul地址
                options.Optional = true; // 2、配置选项
                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                options.OnLoadException = exceptionContext =>
                {
                    Console.WriteLine(exceptionContext.Exception.Message);
                    exceptionContext.Ignore = true;
                }; // 4、忽略异常
            });
        }
    }
}
