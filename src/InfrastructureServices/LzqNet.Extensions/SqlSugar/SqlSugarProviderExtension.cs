using LzqNet.Common.Contracts;
using Masa.BuildingBlocks.Data;
using Serilog;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar;

public static class SqlSugarProviderExtension
{

    /// <summary>
    /// CodeFirst
    /// </summary>
    public static ISqlSugarClient UseCodeFirst(this ISqlSugarClient db)
    {
        var loadedAssemblies = MasaApp.GetAssemblies().ToList();

        // 获取所有实现了 IEntity 的非抽象类
        var entityTypes = loadedAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                       typeof(IEntity).IsAssignableFrom(t))
            .ToList();
        foreach (var type in entityTypes)
        {
            try
            {
                db.CodeFirst.InitTablesWithAttr(type);
            }
            catch (Exception ex)
            {
                Log.Information($"UseCodeFirst：{type.FullName}执行失败:原因是:{ex.Message}");
            }
        }
        return db;
    }

    /// <summary>
    /// 种子数据
    /// </summary>
    public static ISqlSugarClient UseSeedData(this ISqlSugarClient db)
    {
        var loadedAssemblies = MasaApp.GetAssemblies().ToList();
        
        // 获取所有实现了 ISeedData<> 的非抽象类
        var seedDataTypes = loadedAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                       t.GetInterfaces().Any(i => i.IsGenericType &&
                                                  i.GetGenericTypeDefinition() == typeof(ISeedData<>)));
        foreach (var type in seedDataTypes)
        {
            try
            {
                // 创建种子数据实例
                var seedDataInstance = Activator.CreateInstance(type);

                if (seedDataInstance == null)
                {
                    Log.Warning($"无法创建 {type.FullName} 的实例");
                    continue;
                }

                // 获取 Execute 方法
                var executeMethod = type.GetMethod("Execute",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (executeMethod != null)
                {
                    // 执行种子数据的同步方法
                    executeMethod.Invoke(seedDataInstance, new object[] { db });
                    Log.Information($"种子数据 {type.FullName} 执行成功");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"UseSeedData {type.FullName} 执行过程中发生错误");
            }
        }
        return db;
    }

    /// <summary>
    /// sql日志
    /// </summary>
    public static SqlSugarProvider UseSqlLog(this SqlSugarProvider db)
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
        return db;
    }

    /// <summary>
    /// 字段审计
    /// </summary>
    /// <param name="db"></param>
    public static SqlSugarProvider UseAuditedField(this SqlSugarProvider db)
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
        return db;
    }

    /// <summary>
    /// 查询过滤
    /// </summary>
    public static SqlSugarProvider UseQueryFilter(this SqlSugarProvider db)
    {
        db.Context.QueryFilter
            .AddTableFilter<IBaseFullEntity>(a => a.IsDeleted == false);
        return db;
    }
}