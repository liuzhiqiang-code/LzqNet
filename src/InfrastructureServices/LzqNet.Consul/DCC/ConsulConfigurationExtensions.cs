using Consul.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace LzqNet.Consul.DCC;

public static class ConsulConfigurationExtensions
{
    public static void AddConsulConfiguration(this IHostApplicationBuilder builder, params List<string> configurationKeys)
    {
        string consul_url = builder.Configuration.GetValue<string>("Consul:Url")
            ?? "http://localhost:8500";
        foreach (var configurationKey in configurationKeys)
        {
            builder.Configuration.AddConsul(configurationKey, options =>
            {
                options.Optional = true;
                options.ReloadOnChange = true;
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); };
            });
        }
    }
}
