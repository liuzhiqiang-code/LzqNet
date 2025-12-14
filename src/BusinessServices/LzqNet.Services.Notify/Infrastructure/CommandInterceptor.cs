using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace LzqNet.Services.Notify.Infrastructure;

/// <summary>
/// EF Core SQL命令拦截器
/// </summary>
public class CommandInterceptor : DbCommandInterceptor
{
    // 拦截异步查询
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"异步查询SQL: {command.CommandText}");
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    // 拦截同步查询
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Console.WriteLine($"同步查询SQL: {command.CommandText}");
        return base.ReaderExecuting(command, eventData, result);
    }

    // 拦截同步非查询SQL执行（INSERT/UPDATE/DELETE等）
    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        Console.WriteLine($"执行非查询SQL: {command.CommandText}");
        return base.NonQueryExecuting(command, eventData, result);
    }

    // 拦截异步非查询SQL执行（INSERT/UPDATE/DELETE等）
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"异步执行非查询SQL: {command.CommandText}");
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }
}