using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using LzqNet.Services.Msm.Domain.Entities;
using LzqNet.Services.Msm.Domain.Repositories;

/*
 * @author : liuzhiqiang
 * @date : 2025-12-21
 * @desc : role_auth
 */
namespace LzqNet.Services.Msm.Infrastructure.Repositories;

public class RoleAuthRepository(ExampleDbContext context, IUnitOfWork unitOfWork)
    : Repository<ExampleDbContext, RoleAuthEntity, long>(context, unitOfWork), IRoleAuthRepository
{
    
}