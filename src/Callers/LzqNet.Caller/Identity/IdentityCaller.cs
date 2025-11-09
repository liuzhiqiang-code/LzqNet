using LzqNet.Caller.Identity.Contracts;
using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.Caller.Identity;
public class IdentityCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://lzqnet.identity:8080";

    public IdentityCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    public async Task<UserLoginViewDto?> Login(UserLoginDto dto)
    {
        var result = await Caller.PostAsync<IdentityResult<UserLoginViewDto>>($"/login", dto);
        return result?.Data ?? null;
    }
}
