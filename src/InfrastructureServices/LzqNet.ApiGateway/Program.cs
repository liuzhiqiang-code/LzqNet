using Daily.Carp.Extension;
using LzqNet.ApiGateway.Extensions;
using LzqNet.ApiGateway.Extensions.HealthCheck;
using LzqNet.DCC;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();
builder.AddCustomSerilog();

builder.Services.AddMapster();

// Add services to the container.
builder.AddCustomHealthChecks();
builder.AddCustomHealthChecksUI();
builder.Services.AddOpenApi();
builder.AddCustomSwaggerUI();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 全局限流中间件配置
builder.AddCustomRateLimiter("customPolicy");
builder.AddCustomCarp();
builder.Services.AddHttpContextAccessor();
builder.AddCustomMasaRegistrationCaller();
builder.AddCustomMetrics();// 配置遥测中间件
builder.AddCustomResponseCaching();// 配置响应缓存中间件

//builder.AddCustomAuthentication();
//builder.AddCustomAuthorization();

var app = builder.Build();

app.MapOpenApi();
app.MapCustomSwaggerUI();
app.UseCustomMetrics();// 使用遥测中间件

app.UseCors("AllowAll");
app.UseRouting();
// 全局限流中间件
app.UseRateLimiter();
//app.UseCustomAuthentication();
//app.UseCustomAuthorization();
app.UseCustomResponseCaching();// 使用响应缓存中间件
app.MapCustomHealthChecks();
app.MapCustomHealthChecksUI();
// 2. 启用YARP中间件
app.UseCarp(); // 必须放在路由映射之后

// 3. 基础路由示例
app.MapGet("/", () => "网关运行中 (YARP)").AllowAnonymous();

app.Run();