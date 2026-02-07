using LzqNet.Extensions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.DingtalkMessage.Consumer;

public class LzqNetDingtalkMessageConsumerModule : LModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<DingtalkMessageConsumer>();
    }
}
