using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Dispatcher.IntegrationEvents;
using Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq.Publisher.Outbox;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq.Publisher;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseRabbitMq(this IDispatcherOptions options)
    {
        options.Services.AddOptions<RabbitMqOptions>().BindConfiguration("RabbitMq");
        options.Services.TryAddSingleton<IPublisher, Publisher>();
        return options;
    }

    public static IDispatcherOptions UseEventLog(this IDispatcherOptions options)
    {
        options.Services.TryAddTransient<IIntegrationEventLogService, IntegrationEventLogService>();
        options.Services.BuildServiceProvider();
        return options;
    }
}