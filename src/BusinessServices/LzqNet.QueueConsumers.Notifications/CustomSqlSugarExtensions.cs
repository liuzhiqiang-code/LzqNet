using Serilog;
using SqlSugar;

namespace LzqNet.QueueConsumers.Notifications;

public static class CustomSqlSugarExtensions
{
    public static void AddCustomSqlsugar(this IHostApplicationBuilder builder)
    {
        Log.Information("Start AddCustomSqlsugar");

        var connectionString = builder.Configuration.GetConnectionString("MsmConnection")
            ?? throw new InvalidOperationException($"未找到配置项:ConnectionStrings:MsmConnection");
        SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = SqlSugar.DbType.MySql,
            ConnectionString = connectionString,
            IsAutoCloseConnection = true,
        },
        db =>
        {
            //每次上下文都会执行

            //获取IOC对象不要求在一个上下文
            //var log=s.GetService<Log>()

            //获取IOC对象要求在一个上下文
            //var appServive = s.GetService<IHttpContextAccessor>();
            //var log= appServive?.HttpContext?.RequestServices.GetService<Log>();

            db.Aop.OnLogExecuting = (sql, pars) =>
            {

            };
        });
        builder.Services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }
}

