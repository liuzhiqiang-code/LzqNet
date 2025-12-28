using LzqNet.Caller.Common.Contracts;
using LzqNet.Caller.Msm.Contracts.Menu;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_menu")]
public class MenuEntity : BaseFullEntity
{
    [Column("pid")]
    public long? Pid { get; set; }
    [Column("auth_code")]
    public string? AuthCode { get; set; }
    [Column("component")]
    public string? Component { get; set; }
    [Column("meta")]
    public MenuMeta? Meta { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("status")]
    public EnableStatusEnum Status { get; set; } = EnableStatusEnum.Enabled;
    [Column("path")]
    public string? Path { get; set; }
    [Column("redirect")]
    public string? Redirect { get; set; }
    [Column("type")]
    public string Type { get; set; }
}