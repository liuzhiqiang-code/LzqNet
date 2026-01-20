using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Entities.Modeling;

namespace LzqNet.Services.Msm.Domain.Repositories;

public interface IModelingRepository : ISqlSugarRepository<ModelingEntity>, ITransientDependency
{

}