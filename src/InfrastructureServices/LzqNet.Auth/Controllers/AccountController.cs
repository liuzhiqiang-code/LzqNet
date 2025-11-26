using Duende.IdentityModel.Client;
using LzqNet.Auth.Infrastructure;
using LzqNet.Auth.Models;
using LzqNet.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;

namespace LzqNet.Auth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly JwtOption _jwtOption;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(
        IOptions<JwtOption> option,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IHttpClientFactory httpClientFactory)
    {
        _jwtOption = option.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost("Register")]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = "Model is invalid", details = ModelState });
        }

        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok(new { message = "Registration successful!" });
        }

        return BadRequest(new { errors = result.Errors });
    }

    /// <summary>
    /// 登录并获取 Token（Resource Owner Password 方式）
    /// </summary>
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid input.", errors = ModelState.Values });
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return BadRequest(new { message = "Invalid login attempt." });
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_jwtOption.Authority);
        var tokenEndpoint = "/connect/token"; // IdentityServer Token 端点

        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "register.client" },
                { "client_secret", "secret" },
                { "grant_type", "password" },
                { "username", model.UserName },
                { "password", model.Password },
                { "scope", "common" }
            });

        var response = await client.PostAsync(tokenEndpoint, requestContent);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { message = "Failed to obtain token from IdentityServer" });
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenViewModel>();
        return Ok(AdminResult.Success(tokenResponse));
    }

    /// <summary>
    /// 退出登录（仅用于前端管理）
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully!" });
    }

    /// <summary>
    /// 获取客户端凭证模式的 Token
    /// </summary>
    [HttpPost("ClientToken")]
    public async Task<IActionResult> GetClientToken(ClientModel model)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_jwtOption.Authority);
        var tokenEndpoint = "/connect/token"; // IdentityServer Token 端点

        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", model.ClientId },
                { "client_secret", model.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scope", "common" }
            });

        var response = await client.PostAsync(tokenEndpoint, requestContent);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { message = "Failed to obtain token from IdentityServer" });
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenViewModel>();
        return Ok(AdminResult.Success(tokenResponse));
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("UserInfo")]
    public async Task<IActionResult> UserInfo()
    {
        // 从 Claim 中获取用户ID（需确保 Token 包含 sub Claim）
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        string[] roles = User.FindFirstValue(ClaimTypes.Role)?.Split(",") ?? [];

        // 通过 UserManager 查询用户
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(AdminResult.Success(new {
            user.Id,
            RealName = user.UserName,
            user.UserName,
            user.Email,
            Roles = roles
        }));
    }
}