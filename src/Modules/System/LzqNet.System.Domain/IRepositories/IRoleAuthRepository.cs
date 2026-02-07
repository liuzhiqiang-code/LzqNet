using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.System.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.System.Domain.IRepositories;

public interface IRoleAuthRepository : ISqlSugarRepository<RoleAuthEntity>, ITransientDependency
{

}