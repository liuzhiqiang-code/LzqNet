using Google.Protobuf.WellKnownTypes;
using LzqNet.Caller.Auth.Contracts;
using LzqNet.Extensions.Jwt.Contracts;
using LzqNet.Extensions.Tools;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LzqNet.Extensions.Jwt;

public class AuthAppService: ITransientDependency
{
    private HttpContext? _httpContext;
    private JwtOption _options;

    public AuthAppService(IHttpContextAccessor httpContextAccessor,IOptions<JwtOption> options)
    {
        _options = options.Value;
        _httpContext = httpContextAccessor.HttpContext;
    }

    // 生成访问Token
    public TokenViewDto GenAuthenticate(UserInfo user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);

        return new TokenViewDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = _options.AccessExpiration * 60 // 转换为秒
        };
    }

    // 生成访问Token（短期）
    private string GenerateAccessToken(UserInfo user)
    {
        var claim = new Claim[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("UserName", user.UserName),
            new Claim("Roles", user.Roles.ToJson()),
            new Claim("Email", user.Email ?? ""),
            new Claim("Sex", user.Sex.ToString() ?? ""),
            new Claim("datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new Claim("token_type", "access"), // 标记Token类型
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Token唯一标识
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            AesEncryption.AesEncrypt(_options.Secret, "jwt")));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(_options.AccessExpiration),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 生成刷新Token（长期）
    private string GenerateRefreshToken(UserInfo user)
    {
        var claim = new Claim[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("token_type", "refresh"), // 标记为刷新Token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            AesEncryption.AesEncrypt(_options.RefreshSecret ?? _options.Secret, "jwt_refresh")));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(_options.RefreshExpirationDays),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Claims获取当前用户信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    public UserInfoViewDto GetCurrentUser()
    {
        if (_httpContext == null)
            throw new MasaException("_httpContext为空");

        var user = _httpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
            throw new MasaException("用户未认证");

        // 从Claims中提取用户信息
        var userIdClaim = user.FindFirst("UserId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
        var userNameClaim = user.FindFirst("UserName") ?? user.FindFirst(ClaimTypes.Name);
        var rolesClaim = user.FindFirst("Roles");
        var emailClaim = user.FindFirst("Email") ?? user.FindFirst(ClaimTypes.Email);
        var sexClaim = user.FindFirst("Sex");

        if (userIdClaim == null || userNameClaim == null)
            throw new MasaException("用户信息不完整");

        // 解析角色信息
        List<string> roles = new List<string>();
        if (!string.IsNullOrEmpty(rolesClaim?.Value))
        {
            roles = JsonSerializer.Deserialize<List<string>>(rolesClaim.Value) ?? [];
        }

        // 构建返回的DTO
        var userInfoDto = new UserInfoViewDto
        {
            Id = long.Parse(userIdClaim.Value),
            UserName = userNameClaim.Value,
            Email = emailClaim?.Value ?? string.Empty,
            Roles = roles,
            Sex = int.TryParse(sexClaim?.Value, out int sex) ? sex : 0
        };

        return userInfoDto;
    }
}
