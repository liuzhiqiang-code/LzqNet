using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Data.Common;
using System.Text;

namespace LzqNet.Services.Msm.Infrastructure;

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
        WriteSqlLog("异步查询",command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    // 拦截同步查询
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        WriteSqlLog("同步查询", command);
        return base.ReaderExecuting(command, eventData, result);
    }

    // 拦截同步非查询SQL执行（INSERT/UPDATE/DELETE等）
    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        WriteSqlLog("同步非查询",command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    // 拦截异步非查询SQL执行（INSERT/UPDATE/DELETE等）
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        WriteSqlLog("异步非查询",command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void WriteSqlLog(string title, DbCommand command)
    {
        // 1. 构建参数详情字符串
        var parameters = new StringBuilder();
        foreach (DbParameter param in command.Parameters)
        {
            parameters.AppendLine($"  {param.ParameterName}: {param.Value} (Type: {param.DbType}, Size: {param.Size})");
        }

        // 2. 记录完整日志（包含原始SQL、参数详情和可执行SQL）
        Log.Debug($"""
        ====== {title} SQL ======
        原始SQL: {command.CommandText}
        参数详情:
        {parameters}
        可执行SQL: {GenerateExecutableSql(command)}
        执行时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}
        """);
    }

    // 生成可直接执行的SQL（带参数值）
    private string GenerateExecutableSql(DbCommand command)
    {
        var sql = new StringBuilder(command.CommandText);

        foreach (DbParameter param in command.Parameters)
        {
            var paramValue = param.Value switch
            {
                null => "NULL",
                string s => $"'{s.Replace("'", "''")}'", // 处理字符串中的单引号
                DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'", // 格式化日期
                bool b => b ? "1" : "0", // 布尔转数字
                _ => param.Value.ToString() // 其他类型直接转换
            };
            sql.Replace(param.ParameterName, paramValue);
        }

        return sql.ToString();
    }
}