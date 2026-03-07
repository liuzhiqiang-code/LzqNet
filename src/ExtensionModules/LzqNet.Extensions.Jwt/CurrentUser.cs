using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LzqNet.Extensions.Jwt;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private ClaimsPrincipal? _user;

    // 用于依赖注入
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // 用于手动创建
    public CurrentUser()
    {
    }

    private ClaimsPrincipal? User => _user ??= _httpContextAccessor?.HttpContext?.User;

    private string? FindClaimValue(string claimType)
    {
        // 如果有 ClaimsPrincipal，从中获取
        if (User != null)
        {
            return User.FindFirst(claimType)?.Value;
        }

        // 否则从手动设置的属性返回
        return claimType switch
        {
            ClaimTypes.NameIdentifier => _userId,
            ClaimTypes.Name => _userName,
            ClaimTypes.Email => _email,
            "sex" => _sex,
            "sid" => _sid,
            "tenant_id" => _tenantId,
            _ => null
        };
    }

    private List<string>? FindRoleValues()
    {
        if (User != null)
        {
            return User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }
        return _roles;
    }

    // 手动设置的字段
    private string _userId = string.Empty;
    private string? _userName;
    private List<string>? _roles;
    private string? _email;
    private string _sex = "Unknown";
    private string _sid = Guid.NewGuid().ToString();
    private string _tenantId = string.Empty;

    // 属性 - 优先从 ClaimsPrincipal 获取，否则返回手动设置的值
    public string UserId => FindClaimValue(ClaimTypes.NameIdentifier) ?? _userId ?? string.Empty;
    public string? UserName => FindClaimValue(ClaimTypes.Name) ?? _userName;
    public List<string>? Roles => FindRoleValues() ?? _roles;
    public string? Email => FindClaimValue(ClaimTypes.Email) ?? _email;
    public string Sex => FindClaimValue("sex") ?? _sex ?? "Unknown";
    public string Sid => FindClaimValue("sid") ?? _sid ?? string.Empty;
    public string TenantId => FindClaimValue("tenant_id") ?? _tenantId ?? string.Empty;

    // 手动设置值的方法
    public CurrentUser SetUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public CurrentUser SetUserName(string? userName)
    {
        _userName = userName;
        return this;
    }

    public CurrentUser SetRoles(List<string>? roles)
    {
        _roles = roles;
        return this;
    }

    public CurrentUser SetEmail(string? email)
    {
        _email = email;
        return this;
    }

    public CurrentUser SetSex(string sex)
    {
        _sex = sex;
        return this;
    }

    public CurrentUser SetTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }
}