using Yarp.ReverseProxy.Configuration;

namespace LzqNet.ApiGateway;

public interface IHotReloadProxyConfigProvider : IProxyConfigProvider, IDisposable
{
    void UpdateConfig(IProxyConfig newConfig);
}
