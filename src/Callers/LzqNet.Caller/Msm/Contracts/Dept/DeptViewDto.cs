
namespace LzqNet.Caller.Msm.Contracts.Dept;
public class DeptViewDto : DeptDto
{
    public long? Pid { get; set; }
    public string DeptName { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
    public List<DeptViewDto> Children { get; set; }
}
