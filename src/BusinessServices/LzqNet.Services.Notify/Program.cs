using LzqNet.DCC;
using LzqNet.Extensions;
using LzqNet.Extensions.Serilog;
using LzqNet.Services.Notify.Infrastructure;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();
builder.AddCustomSerilog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("NotifyConnection")
    ?? throw new InvalidOperationException($"未找到配置项:ConnectionStrings:NotifyConnection");
builder.Services.AddMasaDbContext<ExampleDbContext>(opt =>
{
    opt.UseSqlite(connectionString);
    opt.AddInterceptors(new CommandInterceptor());
});
builder.AddApplicationServices();

//注册Masa相关服务
builder.Services
   .AddDomainEventBus(options =>
   {
       options.UseEventBus();
       options.UseUoW<ExampleDbContext>();
       options.UseRepository<ExampleDbContext>();
   })
   .AddMasaMinimalAPIs(option => option.MapHttpMethodsForUnmatched = ["Post"]);

var app = builder.Build();

app.MapApplicationServices();

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

app.MapGet("/", [Authorize] () => { return "notify-service"; });
app.Run();
