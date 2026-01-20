using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.Utils.Models;
using SqlSugar;

namespace LzqNet.Extensions.SqlSugar.Repository;

public interface ISqlSugarRepository<TEntity> : ISimpleClient<TEntity> where TEntity : class, new()
{
    Task<PaginatedListBase<TEntity>> GetPaginatedListAsync(PaginatedOptions paginatedOptions);
}
