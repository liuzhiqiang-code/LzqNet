using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Dept;
public class DeptPageSearchDto : RequestPageBase
{
    public long? Pid { get; set; }
    public string? Name { get; set; }
    public EnableStatusEnum? Status { get; set; }
}
