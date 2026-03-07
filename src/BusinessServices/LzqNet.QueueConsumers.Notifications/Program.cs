using LzqNet.DingtalkMessage.Consumer;
using LzqNet.Extensions.DCC;
using LzqNet.Extensions.Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();//.AddCustomConsul();
builder.AddCustomSerilog();

// Add services to the container.
builder.Services.AddHostedService<DingtalkMessageConsumer>();

var app = builder.Build();

app.MapGet("/", () => { return "notifications-queueConsumers"; });

app.Run();