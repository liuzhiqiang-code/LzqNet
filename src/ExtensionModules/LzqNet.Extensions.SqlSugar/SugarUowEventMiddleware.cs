using LzqNet.Common.Attributes;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Reflection;

namespace LzqNet.Extensions.SqlSugar;

public class SugarUowEventMiddleware<TEvent>
    (ILogger<SugarUowEventMiddleware<TEvent>> logger, ISqlSugarClient sqlSugarClient) : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    private readonly ILogger<SugarUowEventMiddleware<TEvent>> _logger = logger;
    private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var typeName = @event.GetType().FullName;
        var eventType = @event.GetType();

        // 判断是否需要事务
        if (!RequiresTransaction(eventType,out UnitOfWorkAttribute? unitOfWorkAttr))
        {
            _logger.LogDebug("----- {EventType} 不需要事务，直接执行", typeName);
            await next();
            return;
        }
        else
        {
            try
            {
                await _sqlSugarClient.AsTenant().BeginTranAsync(unitOfWorkAttr!.IsolationLevel);

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

    /// <summary>
    /// 判断事件是否需要事务
    /// </summary>
    private bool RequiresTransaction(Type eventType, out UnitOfWorkAttribute? unitOfWorkAttr)
    {
        unitOfWorkAttr = null;
        // 外部已经有事务了，不需要再开启新的事务，避免嵌套事务
        if (_sqlSugarClient.Ado.IsAnyTran())
        {
            return false;
        }

        // 命名约定：继承 IQuery<> 的命令不走事务
        if (IsQueryType(eventType))
        {
            return false;
        }

        // 检查 UnitOfWorkAttribute
        unitOfWorkAttr = eventType.GetCustomAttribute<UnitOfWorkAttribute>();
        if (unitOfWorkAttr == null)
            return false;
        return true;
    }

    /// <summary>
    /// 判断类型是否继承 IQuery<> 接口
    /// </summary>
    private bool IsQueryType(Type type)
    {
        // 获取类型实现的所有接口
        var interfaces = type.GetInterfaces();

        // 判断是否有 IQuery<> 类型的接口
        foreach (var @interface in interfaces)
        {
            // 检查接口是否为泛型且名称为 IQuery<>
            if (@interface.IsGenericType &&
                @interface.GetGenericTypeDefinition().Name == "IQuery`1")
            {
                return true;
            }
        }

        return false;
    }
}
