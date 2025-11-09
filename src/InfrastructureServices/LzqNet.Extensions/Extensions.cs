using LzqNet.Extensions.Auth;
using LzqNet.Extensions.HealthCheck;
using LzqNet.Extensions.OAuth;
using Serilog;

namespace LzqNet.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        Log.Information("Start AddApplicationServices");

        builder.AddCustomHealthChecks();
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        builder.AddCustomAuthentication();
        builder.AddCustomAuthorization();
    }

    public static void MapApplicationServices(this WebApplication app)
    {
        Log.Information("Start MapApplicationServices");

        //app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseCustomAuthentication();
        app.UseCustomAuthorization();
        app.MapOpenApi();
        app.MapCustomHealthChecks();
    }
}
