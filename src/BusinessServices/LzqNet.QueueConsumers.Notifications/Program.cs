using LzqNet.DingtalkMessage.Consumer;
using LzqNet.Extensions.DCC;
using LzqNet.Extensions.Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();//.AddLzqConsul();
builder.AddLzqSerilog();

// Add services to the container.
builder.Services.AddHostedService<DingtalkMessageConsumer>();

var app = builder.Build();

app.MapGet("/", () => { return "notifications-queueConsumers"; });

app.Run();