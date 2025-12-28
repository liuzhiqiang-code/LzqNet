using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_role")]
public class RoleEntity : BaseFullEntity
{
    [Column("name")]
    public string Name { get; set; }
    [Column("status")]
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    [Column("remark")]
    public string? Remark { get; set; }
}
