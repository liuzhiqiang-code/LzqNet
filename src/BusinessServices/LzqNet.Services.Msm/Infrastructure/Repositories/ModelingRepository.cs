using LzqNet.Extensions.SqlSugar.Repository;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Entities.Modeling;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class ModelingRepository()
    : SqlSugarRepository<ModelingEntity>(), IModelingRepository
{

}