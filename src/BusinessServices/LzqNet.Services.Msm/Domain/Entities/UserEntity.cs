using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Services.Msm.Domain.Entities;

[Table("msm_user")]
public class UserEntity : BaseFullEntity
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Column("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Column("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Column("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [Column("age")]
    public int? Age { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Column("sex")]
    public int? Sex { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属部门
    /// </summary>
    [Column("dept_id")]
    public long? DeptId { get; set; }

    /// <summary>
    /// 所属角色
    /// </summary>
    [Column("roles")]
    public string? Roles { get; set; }

}