using LzqNet.DingtalkMessage.Consumer;
using LzqNet.Extensions;
using LzqNet.Extensions.DCC;
using LzqNet.Extensions.DCC.Consul;
using LzqNet.Extensions.Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationConfiguration().AddCustomConsul();
builder.AddCustomSerilog();

// Add services to the container.
builder.AddApplicationServices();
builder.Services.AddHostedService<DingtalkMessageConsumer>();

var app = builder.Build();
app.MapApplicationServices();

app.MapGet("/", () => { return "notifications-queueConsumers"; });

app.Run();