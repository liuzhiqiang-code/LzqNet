using LzqNet.Caller.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Extensions;
public static class CallerExtensions
{
    public static void AddCustomCaller(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoRegistrationCaller(
            typeof(CallerExtensions).Assembly,
            typeof(AuthCaller).Assembly,
            typeof(ApiGatewayCaller).Assembly
            );
        builder.Services.AddLocalDistributedLock();
        builder.AddCustomRedisCache();
    }
}
