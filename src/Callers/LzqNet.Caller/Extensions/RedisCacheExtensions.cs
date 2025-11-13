
using Masa.BuildingBlocks.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class RedisCacheExtensions
{

    public static void AddCustomRedisCache(this IHostApplicationBuilder builder)
    {

        builder.Services.AddDistributedCache(distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache();//使用分布式 Redis 缓存，默认使用本地 `RedisConfig` 节点的配置
        });
    }
}