using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Test.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace LzqNet.Test.Domain.IRepositories;

public interface ITestContentLogRepository : ISqlSugarRepository<TestContentLogEntity>, ITransientDependency
{

}