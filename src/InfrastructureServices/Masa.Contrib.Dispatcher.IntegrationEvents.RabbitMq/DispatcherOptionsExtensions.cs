using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseRabbitMq(this IDispatcherOptions options)
    {
        options.Services.AddOptions<RabbitMqOptions>().BindConfiguration("RabbitMq");
        options.Services.TryAddSingleton<IPublisher, Publisher>();
        return options;
    }
}