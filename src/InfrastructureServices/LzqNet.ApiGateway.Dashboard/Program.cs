using AntDesign.ProLayout;
using LzqNet.ApiGateway.Dashboard.Extensions;
using LzqNet.Caller.Extensions;
using LzqNet.DCC;
using LzqNet.DCC.Const;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration(DCCPathConst.COMMON, DCCPathConst.GATEWAY);
builder.Services.AddOptions<JwtClientOption>().BindConfiguration("JwtClient")
    .Validate(setting =>
        !string.IsNullOrWhiteSpace(setting.ClientId) &&
        !string.IsNullOrWhiteSpace(setting.ClientSecret),
        "JwtClient配置有误");

// 添加 HttpClient 服务
builder.Services.AddHttpClient();
builder.Services.AddAutoInject();
builder.AddCustomCaller();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
builder.Services.AddLocalization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();