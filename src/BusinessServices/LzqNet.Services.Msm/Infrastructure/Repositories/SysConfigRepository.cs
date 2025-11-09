using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class SysConfigRepository : Repository<ExampleDbContext, SysConfigEntity, long>, ISysConfigRepository
{
    public SysConfigRepository(ExampleDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}