using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

namespace LzqNet.Services.Msm.Domain.Entities;

public class RoleEntity : FullEntity<long, long>
{
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
}
