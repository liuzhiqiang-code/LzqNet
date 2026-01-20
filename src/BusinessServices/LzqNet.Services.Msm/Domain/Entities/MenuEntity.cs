using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.Menu;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_menu")]
public class MenuEntity : BaseFullEntity
{
    [SugarColumn(ColumnName = "pid")]
    public long? Pid { get; set; }

    [SugarColumn(ColumnName = "auth_code")]
    public string? AuthCode { get; set; }

    [SugarColumn(ColumnName = "component")]
    public string? Component { get; set; }

    [SugarColumn(ColumnName = "meta", IsJson = true)]
    public MenuMeta? Meta { get; set; }

    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "status")]
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;

    [SugarColumn(ColumnName = "path")]
    public string? Path { get; set; }

    [SugarColumn(ColumnName = "redirect")]
    public string? Redirect { get; set; }

    [SugarColumn(ColumnName = "type")]
    public string Type { get; set; }
}