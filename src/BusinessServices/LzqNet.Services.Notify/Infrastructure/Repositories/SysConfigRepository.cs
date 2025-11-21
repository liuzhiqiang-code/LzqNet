using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Notify.Infrastructure;
using LzqNet.Services.Notify.Domain.Entities;
using LzqNet.Services.Notify.Domain.Repositories;

namespace LzqNet.Services.Notify.Infrastructure.Repositories;

public class SysConfigRepository : Repository<ExampleDbContext, SysConfigEntity, long>, ISysConfigRepository
{
    public SysConfigRepository(ExampleDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}