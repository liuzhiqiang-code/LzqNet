using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author : liuzhiqiang
 * @date : 2025-12-21
 * @desc : role_auth
 */
namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_role_auth")]
public class RoleAuthEntity : BaseFullEntity
{
    /// <summary>
    /// RoleId
    /// </summary>
    [Column("role_id")]
    public long RoleId { get; set; }

    /// <summary>
    /// MenuId
    /// </summary>
    [Column("menu_id")]
    public long MenuId { get; set; }
          
}