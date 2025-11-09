using Masa.Contrib.Service.Caller.HttpClient;

namespace LzqNet.Caller;
public class MsmServiceCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6025";

    public MsmServiceCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    //public async Task<List<Order>> GetListAsync()
    //{
    //    return (await Caller.GetAsync<List<Order>>($"api/v1/orders/querylist"))!;
    //}
}