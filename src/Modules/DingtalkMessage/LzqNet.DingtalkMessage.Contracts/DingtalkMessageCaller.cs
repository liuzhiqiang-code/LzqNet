using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.DingtalkMessage.Contracts;

public class DingtalkMessageCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6025";

    public DingtalkMessageCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}