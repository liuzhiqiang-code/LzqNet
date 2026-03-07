using SqlSugar;

namespace LzqNet.Extensions.SqlSugar.Repository;

public interface ISqlSugarRepository<TEntity> : ISimpleClient<TEntity> where TEntity : class, new()
{
}
