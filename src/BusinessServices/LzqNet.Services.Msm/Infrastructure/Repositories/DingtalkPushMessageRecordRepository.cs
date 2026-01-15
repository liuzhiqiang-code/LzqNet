using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class DingtalkPushMessageRecordRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, DingtalkPushMessageRecordEntity, long>(context, unitOfWork), IDingtalkPushMessageRecordRepository
{

}