using LzqNet.Caller.Auth;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Service.Caller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace LzqNet.Caller.Common;
public class ClientAuthenticationService : IAuthenticationService
{
    private readonly IDistributedLock _distributedLock;
    private readonly AuthCaller _authCaller;
    private JwtClientOption _jwtClientOption;
    private readonly IDistributedCacheClient _distributedCacheClient;

    public ClientAuthenticationService(IServiceProvider serviceProvider)
    {
        _distributedLock = serviceProvider.GetRequiredService<IDistributedLock>();
        _authCaller = serviceProvider.GetRequiredService<AuthCaller>();
        _jwtClientOption = serviceProvider.GetRequiredService<IOptions<JwtClientOption>>().Value;
        _distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClient>();
    }

    public async Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        var token = await GetTokenAsync();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string> GetTokenAsync()
    {
        using (var lockObj = _distributedLock.TryGet("GetClientTokenAsync"))
        {
            if (lockObj != null)
            {
                var token = _distributedCacheClient.Get<string>("ClientToken");
                if (!string.IsNullOrEmpty(token))
                    return token;

                // 实际项目中替换为您的认证API调用
                var tokenResponse = await _authCaller.GetClientToken(_jwtClientOption);
                if (tokenResponse == null)
                    throw new InvalidOperationException("Failed to obtain access token.");
                token = tokenResponse.AccessToken;
                _distributedCacheClient.Set("ClientToken", token, TimeSpan.FromMinutes(30));

                return token;
            }
            return "is locked";
        }
    }
}