using SqlSugar;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.RabbitMq.Publisher.Outbox;

public class IntegrationEventLogService(ILogger<IntegrationEventLogService>? logger, ISqlSugarClient sqlSugarClient) : IIntegrationEventLogService
{
    private readonly ILogger<IntegrationEventLogService>? _logger = logger;
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

    public async Task<List<Guid>> BulkMarkEventAsFailedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default)
    {
        var failedEventIds = new List<Guid>();

        await BulkUpdateEventStatus(eventIds, IntegrationEventStates.PublishedFailed, eventLogs =>
        {
            eventLogs.ForEach(eventLog =>
            {
                if (eventLog.State != IntegrationEventStates.InProgress)
                {
                    _logger?.LogWarning(
                        "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                        IntegrationEventStates.PublishedFailed, eventLog.State, eventLog.Id);
                    failedEventIds.Add(eventLog.EventId);
                }
            });
        }, cancellationToken);

        return failedEventIds;
    }

    public async Task<List<Guid>> BulkMarkEventAsInProgressAsync(IEnumerable<Guid> eventIds, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        var failedEventIds = new List<Guid>();

        await BulkUpdateEventStatus(eventIds, IntegrationEventStates.InProgress, eventLogs =>
        {
            eventLogs.ForEach(eventLog =>
            {
                if (eventLog.State is IntegrationEventStates.InProgress or IntegrationEventStates.PublishedFailed &&
                    (DateTime.UtcNow - eventLog.ModificationTime).TotalSeconds < minimumRetryInterval)
                {
                    _logger?.LogInformation(
                        "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}, Multitasking execution error, waiting for the next retry",
                        IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                    failedEventIds.Add(eventLog.EventId);
                }
                if (eventLog.State != IntegrationEventStates.NotPublished &&
                    eventLog.State != IntegrationEventStates.InProgress &&
                    eventLog.State != IntegrationEventStates.PublishedFailed)
                {
                    _logger?.LogWarning(
                        "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                        IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                    failedEventIds.Add(eventLog.EventId);
                }
            });
        }, cancellationToken);

        return failedEventIds;
    }

    public async Task<List<Guid>> BulkMarkEventAsPublishedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default)
    {
        var failedEventIds = new List<Guid>();

        await BulkUpdateEventStatus(eventIds, IntegrationEventStates.Published, eventLogs =>
        {
            eventLogs.ForEach(eventLog =>
            {
                if (eventLog.State != IntegrationEventStates.InProgress)
                {
                    _logger?.LogWarning(
                        "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                        IntegrationEventStates.Published, eventLog.State, eventLog.Id);
                    failedEventIds.Add(eventLog.EventId);
                }
            });

        }, cancellationToken);

        return failedEventIds;
    }

    public async Task DeleteExpiresAsync(DateTime expiresAt, int batchCount, CancellationToken token = default)
    {
        var eventLogs = await _sqlSugarClient.Queryable<IntegrationEventLogEntity>().Where(e => e.ModificationTime < expiresAt && e.State == IntegrationEventStates.Published)
            .OrderBy(e => e.CreationTime).Take(batchCount).ToListAsync();
        if (eventLogs.Any())
        {
            await _sqlSugarClient.Deleteable(eventLogs).ExecuteCommandAsync(token);
        }
    }

    public async Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        await UpdateEventStatus(eventId, IntegrationEventStates.PublishedFailed, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.PublishedFailed, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.PublishedFailed}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.InProgress, eventLog =>
        {
            if (eventLog.State is IntegrationEventStates.InProgress or IntegrationEventStates.PublishedFailed &&
                (DateTime.UtcNow - eventLog.ModificationTime).TotalSeconds < minimumRetryInterval)
            {
                _logger?.LogInformation(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}, Multitasking execution error, waiting for the next retry",
                    IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.InProgress}, the current State is {eventLog.State}, Id: {eventLog.Id}, Multitasking execution error, waiting for the next retry");
            }
            if (eventLog.State != IntegrationEventStates.NotPublished &&
                eventLog.State != IntegrationEventStates.InProgress &&
                eventLog.State != IntegrationEventStates.PublishedFailed)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.InProgress}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public async Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        await UpdateEventStatus(eventId, IntegrationEventStates.Published, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.Published, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.Published}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(int retryBatchSize, int maxRetryTimes, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        var time = DateTime.UtcNow.AddSeconds(-minimumRetryInterval);
        var result = await _sqlSugarClient.Queryable<IntegrationEventLogEntity>()
            .Where(e => (e.State == IntegrationEventStates.PublishedFailed || e.State == IntegrationEventStates.InProgress) &&
                e.TimesSent <= maxRetryTimes &&
                e.ModificationTime < time)
            .OrderBy(e => e.CreationTime)
            .Take(retryBatchSize)
            .ToListAsync(cancellationToken);

        if (result.Any())
            result.OrderBy(e => e.CreationTime);

        return result.Map<List<IntegrationEventLog>>();
    }

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var result = await _sqlSugarClient.Queryable<IntegrationEventLogEntity>()
            .Where(e => e.State == IntegrationEventStates.NotPublished)
            .OrderBy(e => e.CreationTime)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
        if (result.Any())
            result.OrderBy(e => e.CreationTime);

        return result.Map<List<IntegrationEventLog>>();
    }

    public async Task SaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var eventLogEntry = new IntegrationEventLogEntity()
        {
            EventId = @event.GetEventId(),
            Topic = @event.Topic,
            Content = @event.ToJson(),
            State = IntegrationEventStates.NotPublished,
            TimesSent = 0,
            TransactionId = _sqlSugarClient.ContextID
        };
        await _sqlSugarClient.Insertable(eventLogEntry).ExecuteCommandAsync(cancellationToken);
    }

    private async Task UpdateEventStatus(Guid eventId,
        IntegrationEventStates status,
        Action<IntegrationEventLog>? action = null,
        CancellationToken cancellationToken = default)
    {
        var eventLogEntry =
            await _sqlSugarClient.Queryable<IntegrationEventLogEntity>().Where(e => e.EventId == eventId).FirstAsync(cancellationToken);
        if (eventLogEntry == null)
            throw new ArgumentException(
                $"The local message record does not exist, please confirm whether the local message record has been deleted or other reasons cause the local message record to not be inserted successfully In EventId: {eventId}",
                nameof(eventId));

        action?.Invoke(eventLogEntry.Map<IntegrationEventLog>());

        if (eventLogEntry.State == status)
        {
            return;
        }

        eventLogEntry.State = status;

        if (status == IntegrationEventStates.InProgress)
            eventLogEntry.TimesSent++;

        try
        {
            await _sqlSugarClient.Updateable(eventLogEntry).ExecuteCommandAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(
                ex,
                "Concurrency error, Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                status, eventLogEntry.State, eventLogEntry.Id);
            throw new UserFriendlyException("Concurrency conflict, update exception");
        }
    }

    private async Task BulkUpdateEventStatus(IEnumerable<Guid> eventIds,
        IntegrationEventStates status,
        Action<List<IntegrationEventLog>>? action = null,
        CancellationToken cancellationToken = default)
    {
        var eventLogEntrys =
            await _sqlSugarClient.Queryable<IntegrationEventLogEntity>().Where(e => eventIds.Contains(e.EventId)).ToListAsync();
        if (eventLogEntrys == null || !eventLogEntrys.Any())
            throw new ArgumentException(
                $"The local message record does not exist, please confirm whether the local message record has been deleted or other reasons cause the local message record to not be inserted successfully In EventId: {eventIds}",
                nameof(eventIds));

        action?.Invoke(eventLogEntrys.Map<List<IntegrationEventLog>>());

        foreach (var eventLogEntry in eventLogEntrys)
        {
            if (eventLogEntry.State == status)
            {
                continue;
            }

            eventLogEntry.State = status;

            if (status == IntegrationEventStates.InProgress)
                eventLogEntry.TimesSent++;
        }

        try
        {
            await _sqlSugarClient.Updateable<IntegrationEventLogEntity>(eventLogEntrys).ExecuteCommandAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"Concurrency conflict, update exception. {ex.Message}");
        }
    }
}
