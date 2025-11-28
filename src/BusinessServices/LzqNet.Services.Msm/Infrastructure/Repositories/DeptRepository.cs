using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class DeptRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, DeptEntity, long>(context, unitOfWork), IDeptRepository
{
}