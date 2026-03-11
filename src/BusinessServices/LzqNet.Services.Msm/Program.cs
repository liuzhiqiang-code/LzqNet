using LzqNet.Extensions.DCC;
using LzqNet.Extensions.Serilog;
using LzqNet.Services.Msm.Extensions;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();//.AddLzqConsul();
builder.AddLzqSerilog();

builder.AddApplicationServices();

var app = builder.Build();

app.MapApplicationServices();

if (app.Environment.IsDevelopment())
{
}

app.MapGet("/", [Authorize] () => { return "msm-service"; });
app.Run();