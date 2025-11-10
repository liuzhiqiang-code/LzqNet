using LzqNet.Services.Msm.Infrastructure;
using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.BuildingBlocks.Dispatcher.Events;
using LzqNet.Contracts.Msm.SysConfig.Commands;
using LzqNet.DCC;
using LzqNet.Extensions;
using LzqNet.Extensions.Serilog;
using LzqNet.Services.Msm.Application.CommandHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSerilog();
builder.AddApplicationConfiguration();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MsmConnection")
    ?? throw new InvalidOperationException($"未找到配置项:ConnectionStrings:MsmConnection");
builder.Services.AddMasaDbContext<ExampleDbContext>(opt =>
{
    opt.UseSqlite(connectionString);
});
builder.AddApplicationServices();

//注册Masa相关服务
var assemblies = new[]
{
    typeof(SysConfigCreateCommand).Assembly,
    typeof(SysConfigCommandHandler).Assembly
};
builder.Services
   .AddMapster()
   .AddAutoInject()
   .AddEventBus(assemblies)
   .AddDomainEventBus(options =>
   {
       options.UseEventBus();
       options.UseUoW<ExampleDbContext>();
       options.UseRepository<ExampleDbContext>();
   })
   .AddMasaMinimalAPIs(option => option.MapHttpMethodsForUnmatched = ["Post"]);

var app = builder.Build();

app.UseMasaExceptionHandler(options =>
{
    var exceptionStatusMap = new Dictionary<Type, int>
    {
        [typeof(ArgumentNullException)] = 299,
        [typeof(MasaArgumentException)] = 400,
        // 可继续添加其他异常类型
    };
    //处理自定义异常
    options.ExceptionHandler = context =>
    {
        var statusCode = exceptionStatusMap.TryGetValue(context.Exception.GetType(), out var code)
        ? code
        : 500;
        context.ToResult(context.Exception.Message, statusCode);
    };
});

app.MapApplicationServices();

app.MapMasaMinimalAPIs();

if (app.Environment.IsDevelopment())
{
    #region MigrationDb
    using var context = app.Services.CreateScope().ServiceProvider.GetService<ExampleDbContext>();
    {
        context!.Database.EnsureCreated();
        await ExampleDbContextSeed.SeedAsync(context);
    }
    #endregion
}

app.MapGet("/",[Authorize] () => { return "msm-service"; });
app.Run();