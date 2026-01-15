using LzqNet.Services.Msm.Domain.Entities.Modeling;
using LzqNet.Services.Msm.Domain.Repositories;
using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class ModelingRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, ModelingEntity, long>(context, unitOfWork), IModelingRepository
{

}