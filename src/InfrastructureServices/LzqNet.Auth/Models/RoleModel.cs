namespace LzqNet.Auth.Models;

public class RoleModel
{
    public string Name { get; set; } = string.Empty;

}

public class RoleUpdateModel
{
    public string Name { get; set; } = string.Empty;
    public string NewRoleName { get; set; } = string.Empty;
}
