using LzqNet.Caller.Common.Contracts;
using Serilog;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar;

public class SqlSugarAopService
{
    public void Init(SqlSugarProvider db)
    {
        OnLogExecuting(db);
        QueryFilter(db);
        AuditedField(db);
    }

    /// <summary>
    /// sql日志
    /// </summary>
    public void OnLogExecuting(SqlSugarProvider db)
    {
        //调试SQL事件，可以删掉 (要放在执行方法之前)
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            var nativeSql = UtilMethods.GetNativeSql(sql, pars);
            Log.Debug($"准备执行SQL：【{nativeSql}】");

            //获取无参数化SQL 影响性能只适合调试
            //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
        };

        db.Aop.OnLogExecuted = (sql, pars) =>
        {
            var nativeSql = UtilMethods.GetNativeSql(sql, pars);
            Log.Information($"执行SQL完成：【{nativeSql}】 耗时：{db.Ado.SqlExecutionTime.TotalMilliseconds}ms");
        };

        db.Aop.OnError = (exp) =>//SQL报错
        {
            var nativeSql = UtilMethods.GetNativeSql(exp.Sql, (SugarParameter[])exp.Parametres);
            Log.Error(exp, $"执行SQL报错：【{nativeSql}】");
        };
    }

    /// <summary>
    /// 字段审计
    /// </summary>
    /// <param name="db"></param>
    public void AuditedField(SqlSugarProvider db)
    {
        db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            //inset生效
            if ("CreationTime,ModificationTime".Split(',').Contains(entityInfo.PropertyName) && entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                entityInfo.SetValue(DateTime.Now);//修改CreateTime字段
                                                  //entityInfo有字段所有参数
            }
            //update生效
            if (entityInfo.PropertyName == "ModificationTime" && entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                entityInfo.SetValue(DateTime.Now);//修改UpdateTime字段
            }
        };
    }

    /// <summary>
    /// 查询过滤
    /// </summary>
    public void QueryFilter(SqlSugarProvider db)
    {
        db.Context.QueryFilter
            .AddTableFilter<IBaseFullEntity>(a => a.IsDeleted == false);
    }
}