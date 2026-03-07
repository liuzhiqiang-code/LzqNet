using LzqNet.Extensions.RabbitMq.Publisher.Outbox;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
using Masa.Contrib.Dispatcher.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
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

    public static IDispatcherOptions UseEventLog(this IDispatcherOptions options)
    {
        options.Services.TryAddTransient<IIntegrationEventLogService, IntegrationEventLogService>();
        options.Services.BuildServiceProvider();
        return options;
    }
}