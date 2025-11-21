using Consul;
using Consul.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;
using Winton.Extensions.Configuration.Consul.Parsers;

namespace LzqNet.Consul.Register;

public static class ConsulRegisterExtensions
{
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
