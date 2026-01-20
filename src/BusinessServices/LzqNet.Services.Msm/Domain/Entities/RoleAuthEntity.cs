using LzqNet.Caller.Common.Contracts;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_role_auth")]
public class RoleAuthEntity : BaseFullEntity
{
    /// <summary>
    /// RoleId
    /// </summary>
    [SugarColumn(ColumnName = "role_id")]
    public long RoleId { get; set; }

    /// <summary>
    /// MenuId
    /// </summary>
    [SugarColumn(ColumnName = "menu_id")]
    public long MenuId { get; set; }

}