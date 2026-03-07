using LzqNet.Extensions.Jwt.Callers.Contracts;

namespace LzqNet.Extensions.Jwt.Services;

public interface IJwtService
{
    TokenViewDto GenerateToken(ICurrentUser user, TimeSpan timeSpan);
}
