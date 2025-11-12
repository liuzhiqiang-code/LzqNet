using LzqNet.Caller.ApiGateway.Contracts;
using Masa.Contrib.Service.Caller.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Caller.Auth;
public class ApiGatewayCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }

    public ApiGatewayCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        BaseAddress = configuration.GetSection("Services:apigateway")
            .Get<string>() ?? throw new InvalidOperationException($"未找到配置项:Services:apigateway");
    }
    public async Task<ProxyConfigModel?> GetConfig()
    {
        var result = await Caller.GetAsync<ProxyConfigModel>($"/api/config");
        return result;
    }
}
