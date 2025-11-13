using LzqNet.ApiGateway.Dashboard.Extensions;
using LzqNet.DCC;
using LzqNet.DCC.Const;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration(DCCPathConst.COMMON, DCCPathConst.GATEWAY);
builder.Services.AddOptions<JwtClientOption>().BindConfiguration("JwtClient")
    .Validate(setting =>
        !string.IsNullOrWhiteSpace(setting.ClientId) &&
        !string.IsNullOrWhiteSpace(setting.ClientSecret),
        "JwtClient配置有误");

builder.AddYarpDashboard();

var app = builder.Build();

app.MapYarpDashboard();

app.Run();