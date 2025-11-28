using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Msm.Contracts.Dept;
public class DeptDto
{
    public long Id { get; set; }
    public long? Pid { get; set; }
    public string DeptName { get; set; }
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    public string? Remark { get; set; }
}
