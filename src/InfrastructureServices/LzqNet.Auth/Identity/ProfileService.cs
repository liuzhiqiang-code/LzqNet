using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using LzqNet.Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LzqNet.Auth.Identity;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var userId = context.Subject.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        ArgumentNullException.ThrowIfNull(userId);
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user!);
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, string.Join(",",roles))
        };

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            // 尝试用 UserName 查找
            var userName = context.Subject.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                var userByName = await _userManager.FindByNameAsync(userName);
                context.IsActive = userByName != null;
                return;
            }

            context.IsActive = false;
            return;
        }

        var user = await _userManager.FindByIdAsync(userId);
        context.IsActive = user != null;
    }
}
