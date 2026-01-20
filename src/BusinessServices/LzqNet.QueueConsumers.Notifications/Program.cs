using LzqNet.DCC;
using LzqNet.Extensions;
using LzqNet.Extensions.Serilog;
using LzqNet.QueueConsumers.Notifications.Consumers;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration();
builder.AddCustomSerilog();

// Add services to the container.
builder.AddApplicationServices();
builder.Services.AddHostedService<DingtalkMessageConsumer>();

var app = builder.Build();
app.MapApplicationServices();

app.MapGet("/", () => { return "notifications-queueConsumers"; });

app.Run();