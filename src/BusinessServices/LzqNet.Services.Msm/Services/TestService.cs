using LzqNet.Caller.Msm.Contracts.Test.Events;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LzqNet.Services.Msm.Services;

public class TestService : ServiceBase
{
    private IEventBus EventBus => GetRequiredService<IEventBus>();
    public TestService() : base("/api/v1/test") { }

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


    [Authorize]
    public async Task<IResult> SendRabbitmqMessage([FromBody] SendRabbitmqMessageEvent @event)
    {
        await EventBus.PublishAsync(@event);
        return Results.Ok();
    }
}