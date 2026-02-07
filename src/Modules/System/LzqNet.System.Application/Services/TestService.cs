using LzqNet.System.Contracts.Test.Events;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LzqNet.System.Application.Services;

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