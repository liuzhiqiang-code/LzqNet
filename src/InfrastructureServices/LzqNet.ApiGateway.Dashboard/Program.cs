using LzqNet.ApiGateway.Dashboard.Extensions;
using LzqNet.DCC;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();
builder.AddYarpDashboard();

var app = builder.Build();

app.MapYarpDashboard();

app.Run();