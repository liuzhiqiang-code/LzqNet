using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Utils.Models;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar.Repository;

/// <summary>
/// 基础仓库
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarRepository<TEntity> : SimpleClient<TEntity>, ISqlSugarRepository<TEntity> where TEntity : class, new()
{
    private readonly ITenant itenant;//多租户事务
    public SqlSugarRepository()
    {
        //itenant = unitOfWork.Tenant;
        base.Context = SqlSugarHelper.Client;
        ////通过特性拿到ConfigId
        //var configId = typeof(TEntity).GetCustomAttribute<TenantAttribute>()?.configId;
        //if (configId != null)
        //{
        //    base.Context = base.AsTenant().GetConnection(configId);
        //}
        //itenant = DbScoped.SugarScope;//设置租户接口
    }

    public async Task<PaginatedListBase<TEntity>> GetPaginatedListAsync(PaginatedOptions paginatedOptions)
    {
        RefAsync<int> total = 0;
        var list = await Context.Queryable<TEntity>().ToPageListAsync(paginatedOptions.Page, paginatedOptions.PageSize, total);
        var totalPages = (int)Math.Ceiling(total.Value / (double)paginatedOptions.PageSize);
        return new PaginatedListBase<TEntity>() 
        {
            Result = list,
            Total = total,
            TotalPages = totalPages,
        };
    }
}