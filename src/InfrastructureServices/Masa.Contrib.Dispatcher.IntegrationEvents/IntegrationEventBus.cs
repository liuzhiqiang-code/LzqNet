// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using SqlSugar;
using System.Diagnostics;

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventBus : IIntegrationEventBus
{
    private readonly Lazy<IEventBus?> _lazyEventBus;

    private IEventBus? EventBus => _lazyEventBus.Value;

    private readonly Lazy<IPublisher> _lazyPublisher;
    private IPublisher Publisher => _lazyPublisher.Value;

    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly IIntegrationEventLogService? _eventLogService;
    private readonly string TraceId = Activity.Current?.TraceId.ToString() ?? string.Empty;

    public IntegrationEventBus(
        IServiceProvider serviceProvider,
        Lazy<IEventBus?> eventBusLazy,
        Lazy<IPublisher> lazyPublisher,
        IIntegrationEventLogService? eventLogService = null,
        ILogger<IntegrationEventBus>? logger = null)
    {
        _lazyEventBus = eventBusLazy;
        _lazyPublisher = lazyPublisher;
        _eventLogService = eventLogService;
        _logger = logger;
        _sqlSugarClient = serviceProvider.GetRequiredService<ISqlSugarClient>();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await PublishIntegrationAsync(integrationEvent, cancellationToken);
        }
        else if (EventBus != null)
        {
            await EventBus.PublishAsync(@event, cancellationToken);
        }
        else
        {
            throw new NotSupportedException(nameof(@event));
        }
    }

    private async Task PublishIntegrationAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent
    {
        var topicName = @event.Topic;
        if (_eventLogService != null)
        {
            _logger?.LogDebug("----- Saving changes and integrationEvent: {IntegrationEventId}", @event.GetEventId());
            await _eventLogService.SaveEventAsync(@event, cancellationToken);
        }
        else
        {
            _logger?.LogDebug(
                "----- Publishing integration event (don't use local message): {IntegrationEventIdPublished} from {TraceId} - ({IntegrationEvent})",
                @event.GetEventId(),
                TraceId ?? string.Empty, @event);

            await Publisher.PublishAsync(topicName, (dynamic)@event, cancellationToken);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
    }
}
