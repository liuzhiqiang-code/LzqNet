using Masa.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Msm.Contracts.Dept;
public class DeptPageSearchDto : RequestPageBase
{
    public long? Pid { get; set; }
    public string? Name { get; set; }
    public EnableStatusEnum? Status { get; set; }
}
