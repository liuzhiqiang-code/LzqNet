using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.Test.Contracts;

public class TestCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6025";

    public TestCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}