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
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var eventBusAssemblyNames = builder.Configuration.GetSection("Masa:EventBusAssemblyNames").Get<string[]>() ?? [];
        var loadedEventBusAssemblies = new List<Assembly>() { entryAssembly };
        foreach (var assembly in eventBusAssemblyNames)
            loadedEventBusAssemblies.Add(Assembly.Load(assembly));

        var callerAssemblyNames = builder.Configuration.GetSection("Masa:CallerAssemblyNames").Get<string[]>() ?? [];
        var loadedCallerAssemblies = new List<Assembly> { entryAssembly };
        foreach (var assembly in callerAssemblyNames)
            loadedCallerAssemblies.Add(Assembly.Load(assembly));

        // 分布式雪花ID生成器
        var redisOptions = builder.Configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>() ?? 
            throw new MasaException("未找到RedisConfig配置");
        builder.Services.AddSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.UseRedis(
                option => option.GetWorkerIdMinInterval = 5000,
                redisOptions);
        });
        // 抽象公用的Masa 框架服务注册
        builder.Services
            .AddMapster()
            .AddAutoInject()
            .AddValidatorsFromAssemblies(loadedEventBusAssemblies)
            .AddEventBus(loadedEventBusAssemblies, eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(ValidatorEventMiddleware<>)))
            .AddLocalDistributedLock()
            .AddDistributedCache(distributedCacheOptions =>
            {
                distributedCacheOptions.UseStackExchangeRedisCache();//使用分布式 Redis 缓存，默认使用本地 `RedisConfig` 节点的配置
            })
            .AddAutoRegistrationCaller(loadedCallerAssemblies)
            .AddMasaMinimalAPIs(options =>
            {
                options.DisableTrimMethodPrefix = true;//禁用移除方法前缀(上方 `Get`、`Post`、`Put`、`Delete` 请求的前缀)
                options.MapHttpMethodsForUnmatched = ["Post"];//当前服务禁用自动注册路由
            });
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
