using Masa.BuildingBlocks.Caching;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Serilog.Core;

namespace LzqNet.Extensions.RedisCache;

public static class RedisCacheExtensions
{

    public static void AddCustomRedisCache(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomRedisCache");

        builder.Services.AddDistributedCache(distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache();//使用分布式 Redis 缓存，默认使用本地 `RedisConfig` 节点的配置
        });
    }
}