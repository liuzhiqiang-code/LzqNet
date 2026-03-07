using LzqNet.Extensions.SqlSugar.Entities;
using SqlSugar;

namespace LzqNet.System.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("msm_role")]
public class RoleEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "name", Length = 100)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "status")]
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;

    [SugarColumn(ColumnName = "remark", Length = 2000)]
    public string? Remark { get; set; }
}
