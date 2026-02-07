using Masa.Utils.Models;

namespace LzqNet.System.Contracts.Dept;
public class DeptPageSearchDto : RequestPageBase
{
    public long? Pid { get; set; }
    public string? Name { get; set; }
    public EnableStatusEnum? Status { get; set; }
}
