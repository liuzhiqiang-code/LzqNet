using LzqNet.DCC.Option;
using Microsoft.AspNetCore.Identity;

namespace LzqNet.Auth.Infrastructure;

public class ApplicationDbContextSeed
{
    internal static async Task SeedAsync(IServiceScope scope, ConfigurationManager configuration)
    {
        // 初始化admin账号
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        List<DefaultAccountOption> defaultAccountOptions =
            configuration.GetSection("DefaultAccounts").Get<List<DefaultAccountOption>>()
            ?? new();

        foreach (var defaultAccount in defaultAccountOptions)
        {
            // 创建admin角色
            if (!await roleManager.RoleExistsAsync(defaultAccount.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(defaultAccount.Role));
            }

            // 创建admin用户
            var adminUser = await userManager.FindByNameAsync(defaultAccount.UserName);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = defaultAccount.UserName,
                    Email = defaultAccount.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, defaultAccount.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, defaultAccount.Role);
                }
            }
        }
    }
}
