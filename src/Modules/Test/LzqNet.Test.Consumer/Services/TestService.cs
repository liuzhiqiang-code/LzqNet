using LzqNet.Test.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Test.Consumer.Services;

public class TestService() : ISingletonDependency
{
    public async Task ProcessHandleAsync(TestEvent @event)
    {
        //do something
    }
}
