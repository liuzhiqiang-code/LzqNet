using LzqNet.Caller.Auth.Contracts;
using LzqNet.Caller.Common;
using LzqNet.Caller.Msm.Contracts.Account;
using Masa.BuildingBlocks.Service.Caller;
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
    protected override void UseHttpClientPost(MasaHttpClientBuilder masaHttpClientBuilder)
    {
        masaHttpClientBuilder.UseAuthentication(serviceProvider =>
            new ApiAuthenticationService(serviceProvider));
    }
    public async Task<TokenViewDto?> Login(UserLoginDto dto)
    {
        var result = await Caller.PostAsync<AdminResult<TokenViewDto>>($"/api/Account/Login", dto);
        return result?.Data;
    }
    public async Task Logout()
    {
        await Caller.PostAsync<AdminResult>($"/api/Account/Logout",null);
    }

    public async Task<UserInfoViewDto?> UserInfo()
    {
        var result = await Caller.GetAsync<AdminResult<UserInfoViewDto>>($"/api/Account/UserInfo");
        return result?.Data;
    }

    public async Task<TokenViewDto?> GetClientToken(JwtClientOption option)
    {
        var result = await Caller.PostAsync<AdminResult<TokenViewDto>>($"/api/Account/ClientToken", option);
        return result?.Data;
    }

    public async Task<AdminResult> AssignRoleToUser(UserRoleModel model)
    {
        return await Caller.PostAsync<AdminResult>($"/api/Role/AssignRoleToUser", model)
            ?? AdminResult.Fail("调试接口失败");
    }
    public async Task<AdminResult> CreateRole(RoleModel model)
    {
        return await Caller.PostAsync<AdminResult>($"/api/Role/Create", model)
            ?? AdminResult.Fail("调试接口失败");
    }
    public async Task<AdminResult> UpdateRole(RoleUpdateModel model)
    {
        return await Caller.PostAsync<AdminResult>($"/api/Role/Update", model)
            ?? AdminResult.Fail("调试接口失败");
    }
    public async Task<AdminResult> DeleteRole(List<RoleModel> input)
    {
        return await Caller.PostAsync<AdminResult>($"/api/Role/Delete", input)
            ?? AdminResult.Fail("调试接口失败");
    }
}
