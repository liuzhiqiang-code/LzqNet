using LzqNet.Common.Contracts;
using SqlSugar;

namespace LzqNet.System.Domain.Entities;

[SugarTable("msm_user")]
public class UserEntity : BaseFullEntity
{
    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(ColumnName = "user_name")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnName = "password")]
    public string Password { get; set; }

    /// <summary>
    /// 姓
    /// </summary>
    [SugarColumn(ColumnName = "surname")]
    public string? Surname { get; set; }

    /// <summary>
    /// 名
    /// </summary>
    [SugarColumn(ColumnName = "given_name")]
    public string? GivenName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnName = "email")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(ColumnName = "phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [SugarColumn(ColumnName = "age")]
    public int? Age { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnName = "sex")]
    public int? Sex { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnName = "remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属部门
    /// </summary>
    [SugarColumn(ColumnName = "dept_id")]
    public long? DeptId { get; set; }

    /// <summary>
    /// 所属角色
    /// </summary>
    [SugarColumn(ColumnName = "roles",IsJson = true)]
    public List<string>? Roles { get; set; }

}