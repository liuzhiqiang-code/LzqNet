using Masa.Contrib.Service.Caller.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Caller.Notify;
public class NotifyCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }
    /// <summary>
    /// 重写UseHttpClientPost方法，使用认证
    /// </summary>
    /// <param name="masaHttpClientBuilder"></param>
    protected override void UseHttpClientPost(MasaHttpClientBuilder masaHttpClientBuilder)
    {
        //masaHttpClientBuilder.UseAuthentication(serviceProvider =>
        //    new ClientAuthenticationService(serviceProvider));
    }

    public NotifyCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        BaseAddress = configuration.GetSection("Services:notify-service")
            .Get<string>() ?? throw new InvalidOperationException($"未找到配置项:Services:notify-service");
    }

}
