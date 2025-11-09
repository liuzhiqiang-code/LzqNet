using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using LzqNet.Auth.Identity;
using LzqNet.Auth.Infrastructure;
using LzqNet.DCC;
using LzqNet.DCC.Option;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();

var services = builder.Services;
var configuration = builder.Configuration;

// 添加Swagger服务
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
});

services.AddDbContext<ApplicationDbContext>(options =>
{
    // 配置上下文使用SQLite
    options.UseSqlite($"Filename={Path.Combine(Path.GetTempPath(), "openiddict-hollastin-server.sqlite3")}");
});

// 注册Identity服务
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // 密码复杂度设置
    options.Password.RequiredLength = 6; // 最小长度
    options.Password.RequireDigit = true; // 需要数字
    options.Password.RequireLowercase = false; // 需要小写字母
    options.Password.RequireUppercase = false; // 需要大写字母
    options.Password.RequireNonAlphanumeric = false; // 需要特殊字符
    options.Password.RequiredUniqueChars = 1; // 唯一字符数
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 添加 IdentityServer 服务
var identityServerBuilder = builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityConfig.Clients)               // 加载客户端配置
    .AddInMemoryApiScopes(IdentityConfig.ApiScopes)           // 加载 API 范围配置
    .AddInMemoryApiResources(IdentityConfig.ApiResources)     // 加载 API 资源配置
    .AddInMemoryIdentityResources(IdentityConfig.IdentityResources);

// 添加自定义验证器
// 手动注册 ResourceOwnerPasswordValidator 和 ProfileService
builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
builder.Services.AddTransient<IProfileService, ProfileService>();

// 使用开发者签名证书仅在开发环境
if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();  // 开发环境使用开发签名证书
}
else
{
    // 在生产环境中应使用真实的签名凭据
    // identityServerBuilder.AddSigningCredential(...);
}

// JWT 身份验证
JwtOption jwtOption = builder.Configuration.GetSection("Jwt")
            .Get<JwtOption>() ?? throw new InvalidOperationException($"未找到配置项:Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtOption.Authority; // 自动从认证服务器元数据获取 issuer，并默认设为 ValidIssuer
        options.Audience = jwtOption.Audience;  // 确保这个与配置的 API 范围一致
        options.RequireHttpsMetadata = jwtOption.RequireHttpsMetadata; // 仅在生产环境强制使用 HTTPS
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // 如果你想允许所有 API 可访问，可以取消注释以下代码：
            // ValidAudience = "yourapi", // 可以从配置文件中设置 Audience
        };

        // 关键配置：API 直接返回 401 而不是重定向
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // 阻止默认重定向
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\": \"Unauthorized\"}");
            }
        };
    });

services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // 启用Swagger中间件
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
    });
    #region MigrationDb
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
    context!.Database.EnsureCreated();
    await ApplicationDbContextSeed.SeedAsync(scope, builder.Configuration);
    #endregion
}

app.UseRouting();

app.UseIdentityServer();
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
