using LzqNet.Auth.Infrastructure;
using LzqNet.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LzqNet.Auth.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleController(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    /// <summary>
    /// 为用户赋予角色
    /// </summary>
    [HttpPost("AssignRoleToUser")]
    public async Task<ActionResult> AssignRoleToUser([FromBody] UserRoleModel model)
    {
        // 验证模型
        if (model == null)
            return BadRequest(AdminResult.Fail("请求参数不能为空"));

        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(AdminResult.Fail("用户名不能为空"));

        if (model.RoleName == null || !model.RoleName.Any())
            return BadRequest(AdminResult.Fail("至少需要一个角色名称"));

        // 检查用户是否存在
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return BadRequest(AdminResult.Fail("用户不存在"));

        // 过滤出有效的角色（存在且用户未拥有）
        var validRoles = new List<string>();
        var errors = new List<string>();

        foreach (var roleName in model.RoleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                errors.Add($"角色 '{roleName}' 不存在");
                continue;
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                errors.Add($"用户已拥有角色 '{roleName}'");
                continue;
            }

            validRoles.Add(roleName);
        }

        if (!validRoles.Any())
        {
            return BadRequest(AdminResult.Fail($"没有有效的角色可赋予: {string.Join("; ", errors)}"));
        }

        // 批量添加角色
        var result = await _userManager.AddToRolesAsync(user, validRoles);

        if (!result.Succeeded)
        {
            return BadRequest(AdminResult.Fail($"角色赋予失败: {string.Join("; ",
                result.Errors.Select(e => e.Description))}"));
        }

        return Ok(AdminResult.Success($"成功赋予角色: {string.Join(", ", validRoles)}",
            errors.Count != 0 ? string.Join("; ", errors) : ""));
    }


    /// <summary>
    /// 新增角色
    /// </summary>
    [HttpPost("Create")]
    public async Task<ActionResult> CreateAsync([FromBody] RoleModel model)
    {
        await _roleManager.CreateAsync(new IdentityRole(model.Name));
        return Ok();
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    [HttpPost("Update")]
    public async Task<ActionResult> UpdateAsync([FromBody] RoleUpdateModel model)
    {
        var role = await _roleManager.FindByNameAsync(model.Name);
        if (role == null)
            return Ok(AdminResult.Fail("角色不存在"));
        role.Name = model.NewRoleName;
        await _roleManager.UpdateAsync(role);
        return Ok(AdminResult.Success());
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpPost("Delete")]
    public async Task<ActionResult> DeleteAsync([FromBody] List<RoleModel> list)
    {
        foreach (var roleModel in list)
        {
            var role = await _roleManager.FindByNameAsync(roleModel.Name);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }
        return Ok(AdminResult.Success());
    }
}