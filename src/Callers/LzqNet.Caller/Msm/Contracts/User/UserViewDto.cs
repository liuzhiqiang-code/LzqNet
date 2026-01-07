
/*
 * @author : liuzhiqiang
 * @date : 2026-1-1
 * @desc : user
 */
namespace LzqNet.Caller.Msm.Contracts.User;

public class UserViewDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 姓
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// 名
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int? Sex { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 所属部门
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 所属角色
    /// </summary>
    public List<long>? Roles { get; set; }

}