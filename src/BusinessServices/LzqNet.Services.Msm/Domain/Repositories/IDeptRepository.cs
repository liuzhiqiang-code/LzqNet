using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using LzqNet.Services.Msm.Domain.Entities;

namespace LzqNet.Services.Msm.Domain.Repositories;

public interface IDeptRepository : IRepository<DeptEntity, long>
{
}