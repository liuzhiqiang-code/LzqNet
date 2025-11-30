namespace LzqNet.Caller.Auth.Contracts;

public class RoleModel
{
    public string Name { get; set; }

}

public class RoleUpdateModel
{
    public string Name { get; set; }
    public string NewRoleName { get; set; }
}
