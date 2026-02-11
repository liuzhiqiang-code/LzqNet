using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.Template.Contracts;

public class TemplateCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6025";

    public TemplateCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}