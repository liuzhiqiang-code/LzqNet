using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using LzqNet.Services.Msm.Domain.Entities;

namespace LzqNet.Services.Msm.Domain.Repositories;

public interface ISysConfigRepository : IRepository<SysConfigEntity, long>
{
}