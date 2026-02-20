using FluentValidation;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
using Masa.Contrib.Data.IdGenerator.Snowflake;
using Serilog;
using System.Reflection;
using System.Text;
using LzqNet.Extensions.SqlSugar;
using LzqNet.Extensions.RabbitMq.Publisher;

public static class CustomMasaExtensions
{
    public static void AddCustomMasa(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        // 抽象公用的Masa 框架服务注册
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var assemblyNames = configuration.GetSection("Masa:AssemblyNames").Get<string[]>() ?? [];
        var loadedAssemblies = new List<Assembly> { entryAssembly }
            .Concat(assemblyNames.Select(Assembly.Load))
            .ToList();

        builder.Services
            .AddMapster()
            .AddAutoInject(loadedAssemblies)
            .AddCustomMasaEventBus(loadedAssemblies)
            .AddCustomMasaIntegrationEventBus()
            .AddLocalDistributedLock()
            .AddDistributedCache(distributedCacheOptions =>
            {
                distributedCacheOptions.UseStackExchangeRedisCache();//使用分布式 Redis 缓存，默认使用本地 `RedisConfig` 节点的配置
            })
            .AddCustomMasaSnowflake(builder.Configuration)
            .AddCustomMasaRegistrationCaller(loadedAssemblies)
            .AddEndpointsApiExplorer()
            .AddMasaMinimalAPIs(options =>
            {
                options.DisableTrimMethodPrefix = true;//禁用移除方法前缀(上方 `Get`、`Post`、`Put`、`Delete` 请求的前缀)
                options.MapHttpMethodsForUnmatched = ["Post"];//当前服务禁用自动注册路由
            });
    }

    private static IServiceCollection AddCustomMasaEventBus(this IServiceCollection services, List<Assembly> assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies)
            .AddEventBus(assemblies, eventBusBuilder =>
            {
                eventBusBuilder.UseMiddleware(typeof(ValidatorEventMiddleware<>));
                eventBusBuilder.UseMiddleware(typeof(SugarUowEventMiddleware<>));
            });
        return services;
    }

    // 注册RabbitMq集成事件总线
    private static IServiceCollection AddCustomMasaIntegrationEventBus(this IServiceCollection services)
    {
        services.AddIntegrationEventBus(option =>
        {
            option.UseRabbitMq();
        });
        return services;
    }

    private static IServiceCollection AddCustomMasaRegistrationCaller(this IServiceCollection services, List<Assembly> assemblies)
    {
        services.AddAutoRegistrationCaller(assemblies);
        return services;
    }
    private static IServiceCollection AddCustomMasaSnowflake(this IServiceCollection services, IConfiguration configuration)
    {
        // 分布式雪花ID生成器
        var redisOptions = configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>();
        if (redisOptions != null)
        {
            services.AddSnowflake(distributedIdGeneratorOptions =>
            {
                distributedIdGeneratorOptions.UseRedis(
                    option => option.GetWorkerIdMinInterval = 5000,
                    redisOptions);
            });
        }
        return services;
    }

    public static void UseCustomMasaExceptionHandler(this WebApplication app)
    {
        app.UseMasaExceptionHandler(options =>
        {
            var exceptionStatusMap = new Dictionary<Type, int>
            {
                [typeof(UserFriendlyException)] = 200,
                [typeof(MasaArgumentException)] = 400,
                [typeof(MasaValidatorException)] = 298,
                // 可继续添加其他异常类型
            };
            //处理自定义异常
            options.ExceptionHandler = context =>
            {
                Log.Error(GetFullExceptionMessage(context.Exception), "发生未处理的异常");

                var statusCode = exceptionStatusMap.TryGetValue(context.Exception.GetType(), out var code)
                ? code
                : 500;
                context.ToResult(AdminResult.Fail(context.Exception.Message, statusCode).ToJson(), statusCode);
            };
        });
    }
    private static string GetFullExceptionMessage(Exception ex)
    {
        var sb = new StringBuilder();
        var currentEx = ex;
        var indent = 0;

        while (currentEx != null)
        {
            sb.AppendLine($"{new string(' ', indent)}异常类型: {currentEx.GetType().Name}");
            sb.AppendLine($"{new string(' ', indent)}异常消息: {currentEx.Message}");
            sb.AppendLine($"{new string(' ', indent)}堆栈跟踪: {currentEx.StackTrace}");
            sb.AppendLine();

            currentEx = currentEx.InnerException;
            indent += 2;
        }

        return sb.ToString();
    }
}
