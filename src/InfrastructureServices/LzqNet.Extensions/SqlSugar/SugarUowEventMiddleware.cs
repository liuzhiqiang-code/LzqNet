using Masa.BuildingBlocks.Dispatcher.Events;

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

        // 避免嵌套事务，如果已有事务，则直接执行下一个中间件，否则默认一个执行的事件为一个事务单元
        if (SqlSugarHelper.Client.Ado.IsAnyTran())
        {
            await next();
        }
        else
        {
            try
            {
                await SqlSugarHelper.Client.BeginTranAsync();

                await next();

                await SqlSugarHelper.Client.CommitTranAsync();

                // 不同的事务是不同的ContextID
                _logger.LogInformation("----- {CommandType} CommitTranAsync {ContextID}", typeName, SqlSugarHelper.Client.ContextID);
            }
            catch (Exception)
            {
                await SqlSugarHelper.Client.RollbackTranAsync();

                _logger.LogInformation("----- {CommandType} RollbackTranAsync {ContextID}", typeName, SqlSugarHelper.Client.ContextID);
                throw;
            }
        }
    }
}
