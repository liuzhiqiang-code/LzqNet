using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.System.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.System.Domain.IRepositories;

public interface ISysConfigRepository : ISqlSugarRepository<SysConfigEntity>, ITransientDependency
{

}