namespace LzqNet.Extensions.DCC;

public static class ConfigurationExtensions
{
    public static IHostApplicationBuilder AddApplicationConfiguration(this IHostApplicationBuilder builder)
    {
        // 引入项目根目录下的 appsettings.json 文件
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        // 引入环境相关的 appsettings.{Environment}.json 文件（如 appsettings.Development.json）
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // 相对于应用根目录的文件夹
        // 递归获取所有子文件夹中的 JSON 文件
        string configurationDirectory = Path.Combine(AppContext.BaseDirectory, "configuration");
        if (Directory.Exists(configurationDirectory))
        {
            var jsonFiles = Directory.GetFiles(configurationDirectory, "*.json", SearchOption.AllDirectories);
            jsonFiles = jsonFiles
                .OrderBy(f => Path.GetDirectoryName(f)?.Replace(configurationDirectory, "").Count(c => c == Path.DirectorySeparatorChar) ?? 0)
                .ThenBy(f => Path.GetFileName(f))
                .ToArray();

            foreach (var file in jsonFiles)
            {
                try
                {
                    Console.WriteLine($"Loading configuration file: {file}");
                    builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);
                }
                catch (Exception ex)
                {
                    // 记录错误但不中断启动
                    Console.WriteLine($"Error loading configuration file {file}: {ex.Message}");
                }
            }
        }
        return builder;
    }
}
