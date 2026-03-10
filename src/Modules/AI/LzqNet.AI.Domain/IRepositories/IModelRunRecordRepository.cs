using LzqNet.AI.Domain.Entities;
using LzqNet.Extensions.SqlSugar.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.AI.Domain.IRepositories;

public interface IModelRunRecordRepository : ISqlSugarRepository<ModelRunRecordEntity>, ITransientDependency
{

}