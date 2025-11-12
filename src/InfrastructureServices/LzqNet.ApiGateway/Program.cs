using LzqNet.ApiGateway;
using LzqNet.ApiGateway.Extensions;
using LzqNet.ApiGateway.Extensions.HealthCheck;
using LzqNet.ApiGateway.Services;
using LzqNet.DCC;
using Yarp.ReverseProxy.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSerilog();
builder.AddApplicationConfiguration();

builder.Services.AddMapster();

// Add services to the container.
builder.AddCustomHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 全局限流中间件配置
builder.AddCustomRateLimiter("customPolicy");
builder.AddCustomYarp();
builder.Services.AddHttpContextAccessor();
builder.AddCustomMetrics();// 配置遥测中间件
builder.AddCustomResponseCaching();// 配置响应缓存中间件

builder.AddCustomAuthentication();
builder.AddCustomAuthorization("customPolicy");

var app = builder.Build();

app.UseCors("AllowAll");
app.MapOpenApi();
app.UseCustomMetrics();// 使用遥测中间件

// 使用 CORS 中间件时，必须在 UseResponseCaching 之前调用 UseCors。
// app.UseCors();
app.UseRouting();
app.UseCustomResponseCaching();// 使用响应缓存中间件
app.UseCustomAuthentication();
app.UseCustomAuthorization();

app.MapCustomHealthChecks();
// 全局限流中间件
app.UseRateLimiter();
// 2. 启用YARP中间件
app.MapReverseProxy(); // 必须放在路由映射之后

// 3. 基础路由示例
app.MapGet("/", () => "网关运行中 (YARP)").AllowAnonymous();

app.MapYarpApi();
app.Run();