using Masa.BuildingBlocks.Dispatcher.Events;
using System.Diagnostics.CodeAnalysis;

namespace LzqNet.Extensions.SqlSugar;

public class SugarUowEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger<SugarUowEventMiddleware<TEvent>> _logger;

    public SugarUowEventMiddleware(ILogger<SugarUowEventMiddleware<TEvent>> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var typeName = @event.GetType().FullName;
        try
        {
            await SqlSugarHelper.Client.AsTenant().BeginTranAsync();
            _logger?.LogDebug("----- BeginTranAsync {CommandType}", typeName);
            await next();
            await SqlSugarHelper.Client.AsTenant().CommitTranAsync();
            _logger?.LogDebug("----- CommitTranAsync {CommandType}", typeName);
        }
        catch (MasaException ex)
        {
            await SqlSugarHelper.Client.AsTenant().RollbackTranAsync();
            _logger?.LogError("----- RollbackTranAsync {CommandType}", typeName);
            throw;
        }
    }
}
