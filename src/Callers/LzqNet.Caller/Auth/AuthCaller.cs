using LzqNet.Caller.Auth.Contracts;
using LzqNet.Core;
using Masa.Contrib.Service.Caller.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Caller.Auth;
public class AuthCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }

    public AuthCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        BaseAddress = configuration.GetSection("Services:auth")
            .Get<string>() ?? throw new InvalidOperationException($"未找到配置项:Services:auth");
    }
    public async Task<TokenViewDto?> Login(UserLoginDto dto)
    {
        var result = await Caller.PostAsync<AdminResult<TokenViewDto>>($"/api/Account/Login", dto);
        return result?.Data;
    }

    public async Task<TokenViewDto?> GetClientToken(JwtClientOption option)
    {
        var result = await Caller.PostAsync<AdminResult<TokenViewDto>>($"/api/Account/ClientToken", option);
        return result?.Data;
    }
}
