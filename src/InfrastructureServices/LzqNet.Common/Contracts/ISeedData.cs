using SqlSugar;

namespace LzqNet.Common.Contracts;

public interface ISeedData<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    List<TEntity> GetSeedData();

    void Execute(ISqlSugarClient db);
}

public abstract class BaseSeedData<TEntity> : ISeedData<TEntity>
    where TEntity : class, new()
{
    public virtual void Execute(ISqlSugarClient db)
    {
        var data = GetSeedData();
        var tableAny = db.AsTenant().QueryableWithAttr<TEntity>().Any();
        if (data != null && data.Any() && !tableAny)
        {
            db.AsTenant().InsertableWithAttr(data).ExecuteCommand();
        }
    }

    public abstract List<TEntity> GetSeedData();
}