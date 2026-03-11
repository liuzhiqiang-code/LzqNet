using Daily.Carp.Extension;
using LzqNet.ApiGateway.Extensions;
using LzqNet.Extensions.DCC;
using LzqNet.Extensions.DCC.Consul;
using LzqNet.Extensions.HealthCheck;
using LzqNet.Extensions.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration().AddLzqConsul();
builder.AddLzqSerilog();

builder.Services.AddMapster();

// Add services to the container.
builder.AddLzqHealthChecks();
builder.AddLzqHealthChecksUI();
builder.AddLzqSwaggerUI();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 全局限流中间件配置
builder.AddLzqRateLimiter("customPolicy");
builder.AddLzqCarp();
builder.Services.AddHttpContextAccessor();
builder.AddLzqMasa();
builder.AddLzqMetrics();// 配置遥测中间件
builder.AddLzqResponseCaching();// 配置响应缓存中间件

var app = builder.Build();

app.UseOpenApi();
app.MapLzqSwaggerUI();
app.UseLzqMetrics();// 使用遥测中间件

app.UseCors("AllowAll");
app.UseRouting();
// 全局限流中间件
app.UseRateLimiter();
//app.UseCustomAuthentication();
//app.UseCustomAuthorization();
app.UseLzqResponseCaching();// 使用响应缓存中间件
app.MapLzqHealthChecks();
app.MapLzqHealthChecksUI();
// 2. 启用YARP中间件
app.UseCarp(); // 必须放在路由映射之后

// 3. 基础路由示例
app.MapGet("/", () => "网关运行中 (YARP)").AllowAnonymous();

app.Run();