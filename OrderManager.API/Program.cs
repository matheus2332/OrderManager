using OrderManager.Application.Interfaces;
using OrderManager.Application.Services;
using OrderManager.Infrastructure.Messaging;
using OrderManager.Infrastructure.Policies;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddSingleton(sp => new ConnectionFactory
{
    HostName = rabbitConfig.GetValue<string>("HostName"),
    UserName = rabbitConfig.GetValue<string>("UserName"),
    Password = rabbitConfig.GetValue<string>("Password"),
    Port = rabbitConfig.GetValue<int>("Port")
});

// Add services to the container.
builder.Services.AddSingleton<IInventoryService, InMemoryInventoryService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

builder.Services.AddSingleton<OrderExpirationConsumer>();
builder.Services.AddHostedService<OrderExpirationConsumerHostedService>();
builder.Services.AddSingleton<IOrderExpirationPublisher, OrderExpirationPublisher>();
builder.Services.AddSingleton<IReservationPolicy, InMemoryReservationPolicy>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
