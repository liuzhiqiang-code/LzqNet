//using LzqNet.DCC.Consul;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;

//namespace LzqNet.DCC;

//public static class ConfigurationExtensions
//{
//    public static IHostApplicationBuilder AddApplicationConfiguration(this IHostApplicationBuilder builder, params List<string> configurationKeys)
//    {
//        // 引入项目根目录下的 appsettings.json 文件
//        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
//        // 引入环境相关的 appsettings.{Environment}.json 文件（如 appsettings.Development.json）
//        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

//        var settingKeys = builder.Configuration.GetSection("ConfigurationKeys").Get<List<string>>() ?? [];
//        foreach (var key in settingKeys)
//            if(!configurationKeys.Contains(key))
//                configurationKeys.Add(key);

//        foreach (var configurationKey in configurationKeys)
//        {
//            //引入Configuration文件夹所有.json文件，后面可替换成dapr或其他分布式配置中心
//            string configurationDirectory = Path.Combine(AppContext.BaseDirectory, "configuration", configurationKey); // 相对于应用根目录的文件夹
//            if (Directory.Exists(configurationDirectory))
//            {
//                var jsonFiles = Directory.GetFiles(configurationDirectory, "*.json");
//                foreach (var file in jsonFiles)
//                    builder.Configuration.AddJsonFile(file, optional: true);
//            }
//        }

//        // 引入Consul配置中心的配置
//        builder.AddConsulConfiguration(configurationKeys);
//        // 注册Consul服务发现
//        builder.AddConsulRegister();
//        return builder;
//    }
//}
