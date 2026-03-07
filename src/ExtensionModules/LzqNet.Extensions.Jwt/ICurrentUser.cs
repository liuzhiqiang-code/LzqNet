namespace LzqNet.Extensions.Jwt;

public interface ICurrentUser
{
    string UserId { get; }
    string? UserName { get; }
    List<string>? Roles { get; }
    string? Email { get; }
    string Sex { get; }
    string Sid { get; }
    string TenantId { get; }
}
