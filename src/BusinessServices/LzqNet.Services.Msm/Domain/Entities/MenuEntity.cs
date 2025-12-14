using LzqNet.Caller.Msm.Contracts.Menu;
using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

namespace LzqNet.Services.Msm.Domain.Entities;

public class MenuEntity : FullEntity<long, long>
{
    public long? Pid { get; set; }
    public string? AuthCode { get; set; }
    public string? Component { get; set; }
    public MenuMeta? Meta { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Path { get; set; }
    public string? Redirect { get; set; }
    public string Type { get; set; }
}