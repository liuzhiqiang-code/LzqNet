using LzqNet.Template.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Template.Consumer.Services;

public class TemplateService() : ISingletonDependency
{
    public async Task ProcessHandleAsync(TemplateEvent @event)
    {
        //do something
    }
}
