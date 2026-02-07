using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Services.Msm.Domain.Entities;

namespace LzqNet.Services.Msm.Domain.Repositories;

public interface ISysConfigRepository : ISqlSugarRepository<SysConfigEntity>, ITransientDependency
{

}