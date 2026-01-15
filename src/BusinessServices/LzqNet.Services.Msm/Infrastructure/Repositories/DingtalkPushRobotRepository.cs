using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class DingtalkPushRobotRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, DingtalkPushRobotEntity, long>(context, unitOfWork), IDingtalkPushRobotRepository
{

}