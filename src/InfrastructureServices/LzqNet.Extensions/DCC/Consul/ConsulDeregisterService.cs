using Consul;

namespace LzqNet.Extensions.DCC.Consul;
// 单独的服务类
public class ConsulDeregisterService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly string _serviceId;

    public ConsulDeregisterService(IConsulClient consulClient, string serviceId)
    {
        _consulClient = consulClient;
        _serviceId = serviceId;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(_serviceId);
    }
}