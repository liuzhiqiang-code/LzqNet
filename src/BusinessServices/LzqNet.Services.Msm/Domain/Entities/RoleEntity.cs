using LzqNet.Caller.Common.Contracts;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_role")]
public class RoleEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "status")]
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;

    [SugarColumn(ColumnName = "remark")]
    public string? Remark { get; set; }
}
