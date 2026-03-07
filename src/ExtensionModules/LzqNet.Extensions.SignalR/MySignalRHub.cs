using LzqNet.Extensions.Jwt;
using LzqNet.Extensions.SignalR.Models;
using Masa.BuildingBlocks.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LzqNet.Extensions.SignalR
{
    public class MySignalRHub : Hub
    {
        public IServiceProvider serviceProvider { get; set; }

        public ICurrentUser _currentUser { get; set; }

        public string identityId { get; set; }

        public IDistributedCacheClient DistributedCacheClient { get; set; }

       
        public  IHttpContextAccessor _httpContextAccessor { get; set; }

        private readonly SignalRRedisSetting _config;

        public MySignalRHub(IOptionsMonitor<SignalRRedisSetting> config, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            if (serviceProvider != default)
            {
                var currentUser = serviceProvider.GetService<ICurrentUser>();
                if (currentUser != default)
                {
                    _currentUser = currentUser;
                }
                var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
                if (distributedCacheClient != default)
                {
                    DistributedCacheClient = distributedCacheClient;
                }  
            }
            _config = config.CurrentValue;
            if (httpContextAccessor != default)
            {
                _httpContextAccessor = httpContextAccessor;
            }
            identityId = GetIdentityId();
        }

        private string GetIdentityId()
        {
            if (_httpContextAccessor != default && _httpContextAccessor.HttpContext != default)
            {
                _httpContextAccessor = _httpContextAccessor;
                if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("IdentityId"))
                {
                    return _httpContextAccessor.HttpContext.Request.Headers["IdentityId"].FirstOrDefault() ?? "";
                }
                if (_httpContextAccessor.HttpContext.Request.Query["IdentityId"].FirstOrDefault() != default)
                {
                    return _httpContextAccessor.HttpContext.Request.Query["IdentityId"].FirstOrDefault() ?? "";
                }
                return _currentUser.UserId.ToString();
            }
            else
            {
                return _currentUser.UserId.ToString();
            }
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <returns></returns> 
        public override async Task OnConnectedAsync()
        {
            if (_currentUser == default)
            {
                throw new Exception($"用户不存在");
            }
            if (identityId != default && _currentUser.TenantId != default)
            {
                var data = new SignalRCacheDto();
                var json = await DistributedCacheClient.GetAsync<string>(_config.cacheMySignalRKeyName);
                if (!string.IsNullOrEmpty(json))
                {
                    data = await DistributedCacheClient.GetAsync<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                    if (data != default)
                    {
                        var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                        if (tenantData != default)
                        {
                            if (tenantData.Connections.FirstOrDefault(t => t.IdentityId == identityId) != default)
                            {
                                tenantData.Connections.RemoveAll(t => t.IdentityId == identityId);
                            }
                            tenantData.Connections?.Add(new IdentityConnectionDto
                            {
                                IdentityId = identityId,
                                ConnectionId = Context.ConnectionId
                            });
                        }
                        else
                        {
                            data.Items.Add(new SignalRRedisItemDto
                            {
                                TenantId = _currentUser.TenantId,
                                Connections = new List<IdentityConnectionDto> {
                                                          new IdentityConnectionDto{ ConnectionId=Context.ConnectionId, IdentityId=identityId}
                                                            }
                            });
                        }
                    }

                }
                else
                {
                    data.Items.Add(new SignalRRedisItemDto
                    {
                        TenantId = _currentUser.TenantId,
                        Connections = new List<IdentityConnectionDto> {
                                                          new IdentityConnectionDto{ ConnectionId=Context.ConnectionId, IdentityId=identityId}
                                                            }
                    });
                }
                await DistributedCacheClient.SetAsync(_config.cacheMySignalRKeyName, data);
            }
            Console.WriteLine(identityId + "-链接MySignalRHub");
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (identityId != default && _currentUser.TenantId != default)
                {
                    var data = await DistributedCacheClient.GetAsync<SignalRCacheDto>(_config.cacheMySignalRKeyName);
                    if (data != default)
                    {
                        var tenantData = data.Items.FirstOrDefault(t => t.TenantId == _currentUser.TenantId);
                        if (tenantData != default)
                        {
                            if (tenantData.Connections.FirstOrDefault(t => t.IdentityId == identityId) != default)
                            {
                                tenantData.Connections.RemoveAll(t => t.IdentityId == identityId);
                            }
                        }
                        await DistributedCacheClient.SetAsync(_config.cacheMySignalRKeyName, data);
                    }

                }
                Console.WriteLine(identityId + "断开链接MySignalRHub");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public new void Dispose()
        {
            DistributedCacheClient.Remove(_config.cacheMySignalRKeyName);
            base.Dispose(); ;
        }
    }
}
