using Consul;
using Consul.AspNetCore;
using Winton.Extensions.Configuration.Consul;

namespace LzqNet.Extensions.DCC.Consul;

public static class ConsulExtensions
{
    public static void AddCustomConsul(this IHostApplicationBuilder builder)
    {
        var configurationKeys = builder.Configuration.GetSection("ConfigurationKeys").Get<List<string>>() ?? [];
        builder.AddConsulConfiguration(configurationKeys);
        builder.AddConsulRegister();
    }

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

    public static void AddConsulRegister(this IHostApplicationBuilder builder)
    {
        var consulOptions = builder.Configuration.GetSection("Consul").Get<ConsulOptions>()
            ?? throw new InvalidOperationException($"未找到配置项:Consul");
        var consulAddress = $"{consulOptions.ConsulIP}:{consulOptions.ConsulPort}";
        if (!consulAddress.StartsWith("http://") && !consulAddress.StartsWith("https://"))
            consulAddress = "http://" + consulAddress; // 默认使用 HTTP 协议

        // 通过consul提供的注入方式注册consulClient
        builder.Services.AddConsul(options => options.Address = new Uri(consulAddress));

        // 通过consul提供的注入方式进行服务注册
        var httpCheck = new AgentServiceCheck()
        {
            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
            Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
            HTTP = $"http://{consulOptions.IP}:{consulOptions.Port}/health",//健康检查地址
            Timeout = TimeSpan.FromSeconds(10)
        };

        // Register service with consul
        var serviceId = Guid.NewGuid().ToString();
        builder.Services.AddConsulServiceRegistration(options =>
        {
            options.Checks = new[] { httpCheck };
            options.ID = serviceId;
            options.Name = consulOptions.ServiceName;
            options.Address = consulOptions.IP;
            options.Port = consulOptions.Port;
            options.Meta = new Dictionary<string, string>() { { "Weight", consulOptions.Weight.HasValue ? consulOptions.Weight.Value.ToString() : "1" } };
            options.Tags = new[] { $"urlprefix-/{consulOptions.ServiceName}" }; //添加
        });

        // 在Program.cs或启动类中添加
        builder.Services.AddHostedService(provider =>
            new ConsulDeregisterService(
                provider.GetRequiredService<IConsulClient>(),
                serviceId
            ));
    }
}