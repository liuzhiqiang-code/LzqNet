
namespace LzqNet.Caller.Msm.Contracts.Role;
public class RoleViewDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
    public List<long> Permissions { get; set; } = [];
}
