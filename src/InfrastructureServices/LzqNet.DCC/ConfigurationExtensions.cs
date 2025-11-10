using LzqNet.Consul.DCC;
using LzqNet.DCC.Const;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace LzqNet.DCC;

public static class ConfigurationExtensions
{
    public static void AddApplicationConfiguration(this IHostApplicationBuilder builder, params List<string> configurationKeys)
    {
        // 引入项目根目录下的 appsettings.json 文件
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        // 引入环境相关的 appsettings.{Environment}.json 文件（如 appsettings.Development.json）
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        configurationKeys.Add(DCCPathConst.COMMON);
        foreach (var configurationKey in configurationKeys)
        {
            //引入Configuration文件夹所有.json文件，后面可替换成dapr或其他分布式配置中心
            string configurationDirectory = Path.Combine(AppContext.BaseDirectory, "configuration", configurationKey); // 相对于应用根目录的文件夹
            if (Directory.Exists(configurationDirectory))
            {
                var jsonFiles = Directory.GetFiles(configurationDirectory, "*.json");
                foreach (var file in jsonFiles)
                    builder.Configuration.AddJsonFile(file, optional: true);
            }
        }

        // 引入Consul配置中心的配置
        builder.AddConsulConfiguration(configurationKeys);
    }
}
