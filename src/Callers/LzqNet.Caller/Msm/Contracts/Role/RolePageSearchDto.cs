using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Role;
public class RolePageSearchDto : RequestPageBase
{
    public string? Name { get; set; }
    public EnableStatusEnum? Status { get; set; }
}
