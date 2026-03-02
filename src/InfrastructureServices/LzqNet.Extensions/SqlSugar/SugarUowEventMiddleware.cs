using Masa.BuildingBlocks.Dispatcher.Events;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar;

public class SugarUowEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger<SugarUowEventMiddleware<TEvent>> _logger;
    private readonly ISqlSugarClient _sqlSugarClient;

    public SugarUowEventMiddleware(ILogger<SugarUowEventMiddleware<TEvent>> logger, ISqlSugarClient sqlSugarClient)
    {
        _logger = logger;
        _sqlSugarClient = sqlSugarClient;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var typeName = @event.GetType().FullName;

        // 避免嵌套事务，如果已有事务，则直接执行下一个中间件，否则默认一个执行的事件为一个事务单元
        if (_sqlSugarClient.Ado.IsAnyTran())
        {
            await next();
        }
        else
        {
            try
            {
                await _sqlSugarClient.AsTenant().BeginTranAsync();

                await next();

                await _sqlSugarClient.AsTenant().CommitTranAsync();

                // 不同的事务是不同的ContextID
                _logger.LogInformation("----- {CommandType} CommitTranAsync {ContextID}", typeName, _sqlSugarClient.ContextID);
            }
            catch (Exception)
            {
                await _sqlSugarClient.AsTenant().RollbackTranAsync();

                _logger.LogInformation("----- {CommandType} RollbackTranAsync {ContextID}", typeName, _sqlSugarClient.ContextID);
                throw;
            }
        }
    }
}
