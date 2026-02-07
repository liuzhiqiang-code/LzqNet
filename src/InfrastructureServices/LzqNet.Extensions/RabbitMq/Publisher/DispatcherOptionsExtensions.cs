using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Dispatcher.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LzqNet.Extensions.RabbitMq.Publisher;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseRabbitMq(this IDispatcherOptions options)
    {
        options.Services.AddOptions<RabbitMqOptions>().BindConfiguration("RabbitMq");
        options.Services.TryAddSingleton<IPublisher, Publisher>();
        return options;
    }
}