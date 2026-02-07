using LzqNet.Extensions;
using LzqNet.Extensions.DCC;
using LzqNet.Extensions.DCC.Consul;
using LzqNet.Extensions.Serilog;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration().AddCustomConsul();
builder.AddCustomSerilog();

builder.AddApplicationServices();

var app = builder.Build();

app.MapApplicationServices();

if (app.Environment.IsDevelopment())
{
}

app.MapGet("/", [Authorize] () => { return "msm-service"; });
app.Run();