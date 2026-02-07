using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.System.Contracts;

public class SystemCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6025";

    public SystemCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}