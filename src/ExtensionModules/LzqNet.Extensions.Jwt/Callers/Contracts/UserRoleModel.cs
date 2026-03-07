namespace LzqNet.Extensions.Jwt.Callers.Contracts;

public class UserRoleModel
{
    public string UserName { get; set; }
    public List<string>? RoleName { get; set; }
}
