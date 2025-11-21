using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using LzqNet.Services.Notify.Domain.Entities;

namespace LzqNet.Services.Notify.Domain.Repositories;

public interface ISysConfigRepository : IRepository<SysConfigEntity, long>
{
}