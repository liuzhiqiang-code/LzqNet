using Masa.Utils.Models;

namespace LzqNet.System.Contracts.Role;
public class RolePageSearchDto : RequestPageBase
{
    public string? Name { get; set; }
    public EnableStatusEnum? Status { get; set; }
}
