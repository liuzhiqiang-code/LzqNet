
namespace LzqNet.Caller.Msm.Contracts.Dept;
public class DeptViewDto
{
    public long Id { get; set; }
    public long? Pid { get; set; }
    public string Name { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
    public List<DeptViewDto> Children { get; set; }
}
