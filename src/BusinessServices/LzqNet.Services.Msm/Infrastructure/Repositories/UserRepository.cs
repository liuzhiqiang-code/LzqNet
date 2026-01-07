using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class UserRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, UserEntity, long>(context, unitOfWork), IUserRepository
{

}