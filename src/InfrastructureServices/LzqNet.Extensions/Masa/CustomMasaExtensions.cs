using FluentValidation;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
using Masa.Contrib.Data.IdGenerator.Snowflake;
using System.Reflection;

public static class CustomMasaExtensions
{
    public static void AddCustomMasa(this IHostApplicationBuilder builder)
    {
        // 抽象公用的Masa 框架服务注册
        builder.Services
            .AddMapster()
            .AddAutoInject()
            .AddCustomMasaEventBus(builder.Configuration)
            .AddLocalDistributedLock()
            .AddDistributedCache(distributedCacheOptions =>
            {
                distributedCacheOptions.UseStackExchangeRedisCache();//使用分布式 Redis 缓存，默认使用本地 `RedisConfig` 节点的配置
            })
            .AddCustomMasaSnowflake(builder.Configuration)
            .AddCustomMasaRegistrationCaller(builder.Configuration)
            .AddMasaMinimalAPIs(options =>
            {
                options.DisableTrimMethodPrefix = true;//禁用移除方法前缀(上方 `Get`、`Post`、`Put`、`Delete` 请求的前缀)
                options.MapHttpMethodsForUnmatched = ["Post"];//当前服务禁用自动注册路由
            });
    }

    private static IServiceCollection AddCustomMasaEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var eventBusAssemblyNames = configuration.GetSection("Masa:EventBusAssemblyNames").Get<string[]>() ?? [];
        var loadedEventBusAssemblies = new List<Assembly> { entryAssembly }
            .Concat(eventBusAssemblyNames.Select(Assembly.Load))
            .ToList();
        services.AddValidatorsFromAssemblies(loadedEventBusAssemblies)
            .AddEventBus(loadedEventBusAssemblies, eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(ValidatorEventMiddleware<>)));
        return services;
    }

    private static IServiceCollection AddCustomMasaRegistrationCaller(this IServiceCollection services, IConfiguration configuration)
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var callerAssemblyNames = configuration.GetSection("Masa:CallerAssemblyNames").Get<string[]>() ?? [];
        var loadedCallerAssemblies = new List<Assembly> { entryAssembly }
            .Concat(callerAssemblyNames.Select(Assembly.Load))
            .ToList();
        services.AddAutoRegistrationCaller(loadedCallerAssemblies);
         return services;
    }
    private static IServiceCollection AddCustomMasaSnowflake(this IServiceCollection services, IConfiguration configuration)
    {
        // 分布式雪花ID生成器
        var redisOptions = configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>() ??
            throw new MasaException("未找到RedisConfig配置");
        services.AddSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.UseRedis(
                option => option.GetWorkerIdMinInterval = 5000,
                redisOptions);
        });
        return services;
    }

    public static void UseCustomMasaExceptionHandler(this WebApplication app)
    {
        app.UseMasaExceptionHandler(options =>
        {
            var exceptionStatusMap = new Dictionary<Type, int>
            {
                [typeof(ArgumentNullException)] = 299,
                [typeof(MasaArgumentException)] = 400,
                [typeof(MasaValidatorException)] = 400,
                // 可继续添加其他异常类型
            };
            //处理自定义异常
            options.ExceptionHandler = context =>
            {
                var statusCode = exceptionStatusMap.TryGetValue(context.Exception.GetType(), out var code)
                ? code
                : 500;
                context.ToResult(AdminResult.Fail(context.Exception.Message, statusCode).ToJson(),statusCode);
            };
        });
    }
}
