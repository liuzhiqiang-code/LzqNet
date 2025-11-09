using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LzqNet.DCC;

public static class ConfigurationExtensions
{
    public static void AddApplicationConfiguration(this IHostApplicationBuilder builder)
    {
        // 引入项目根目录下的 appsettings.json 文件
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        // 引入环境相关的 appsettings.{Environment}.json 文件（如 appsettings.Development.json）
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


        //引入Configuration文件夹所有.json文件，后面可替换成dapr或其他分布式配置中心
        string configurationDirectory = Path.Combine(AppContext.BaseDirectory, "Configuration"); // 相对于应用根目录的文件夹
        if (Directory.Exists(configurationDirectory))
        {
            var jsonFiles = Directory.GetFiles(configurationDirectory, "*.json");
            foreach (var file in jsonFiles)
                builder.Configuration.AddJsonFile(file, optional: true);
        }
    }
}
