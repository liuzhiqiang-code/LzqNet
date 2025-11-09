using Duende.IdentityModel;
using Duende.IdentityServer.Validation;
using LzqNet.Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LzqNet.Auth.Identity;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        if (!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                return;
            }
            if (!await _userManager.CheckPasswordAsync(user, context.Password))
            {
                return;
            }

            context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password, new Claim[] { new("my_phone", "10086") }); //这里增加自定义信息
            return;
        }
        return;
    }

}
