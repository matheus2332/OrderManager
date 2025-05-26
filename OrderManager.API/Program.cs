using OrderManager.Application.Interfaces;
using OrderManager.Application.Services;
using OrderManager.Infrastructure.Messaging;
using OrderManager.Infrastructure.Policies;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
