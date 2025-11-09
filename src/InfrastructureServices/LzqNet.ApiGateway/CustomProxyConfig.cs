using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace LzqNet.ApiGateway;

public class CustomProxyConfig : IProxyConfig
{
    // 必须实现的接口属性
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }  // 热更新时设为新Token

    // 构造函数
    public CustomProxyConfig(
        List<RouteConfig> routes,
        List<ClusterConfig> clusters)
    {
        Routes = routes.AsReadOnly();
        Clusters = clusters.AsReadOnly();
    }
}
