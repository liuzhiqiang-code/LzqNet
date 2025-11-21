using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LzqNet.Services.Notify.Services;

public class TestService : ServiceBase
{
    private readonly HttpContext _httpContext;

    public TestService()
    {
    }

    [Authorize]
    public IResult GetClaims([FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var claims = httpContextAccessor.HttpContext!.User.Claims.Select(claim =>
        {
            return new
            {
                claim.Type,
                claim.Value
            };
        });
        return Results.Ok(claims);
    }
}