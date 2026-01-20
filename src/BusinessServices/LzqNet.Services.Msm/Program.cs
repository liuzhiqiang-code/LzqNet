using LzqNet.DCC;
using LzqNet.Extensions;
using LzqNet.Extensions.Serilog;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();
builder.AddCustomSerilog();

builder.AddApplicationServices();

var app = builder.Build();

app.MapApplicationServices();

if (app.Environment.IsDevelopment())
{
}

app.MapGet("/", [Authorize] () => { return "msm-service"; });
app.Run();