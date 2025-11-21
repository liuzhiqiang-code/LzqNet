using LzqNet.Contracts.Notify.QQMail.Commands;
using LzqNet.DCC;
using LzqNet.DCC.Const;
using LzqNet.Extensions;
using LzqNet.Extensions.Serilog;
using LzqNet.Services.Notify.Infrastructure;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration(DCCPathConst.COMMON, DCCPathConst.MSM_SERVICE);
builder.AddCustomSerilog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("NotifyConnection")
    ?? throw new InvalidOperationException($"未找到配置项:ConnectionStrings:NotifyConnection");
builder.Services.AddMasaDbContext<ExampleDbContext>(opt =>
{
    opt.UseSqlite(connectionString);
});
builder.AddApplicationServices();

//注册Masa相关服务
var assemblies = new[]
{    
    typeof(Program).Assembly,
    typeof(QQMailSendCommand).Assembly,
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

app.UseMiddleware<HttpLoggingMiddleware>();
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

app.MapGet("/", [Authorize] () => { return "notify-service"; });
app.MapPost("/api/v1/Mails/QQMailSend", async ([FromServices] IEventBus eventBus, HttpRequest request) =>
{
    var form = await request.ReadFormAsync();
    await eventBus.PublishAsync(new QQMailSendCommand { 
        
    
    });
}).DisableAntiforgery();
app.Run();
