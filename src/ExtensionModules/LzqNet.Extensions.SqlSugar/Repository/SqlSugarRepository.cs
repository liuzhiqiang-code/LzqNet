using Masa.BuildingBlocks.Data;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar.Repository;

/// <summary>
/// 基础仓库
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarRepository<TEntity> : SimpleClient<TEntity>, ISqlSugarRepository<TEntity> where TEntity : class, new()
{
    public SqlSugarRepository()
    {
        base.Context = MasaApp.GetRequiredService<ISqlSugarClient>();
    }
}