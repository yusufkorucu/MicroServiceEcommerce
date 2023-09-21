using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer.Payment;
using Order.API.Moduls;
using Shared;
using Shared.Events.Stock;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderAPIDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<PaymentCompletedEventConsumer>();
    conf.AddConsumer<PaymentFailedEventConsumer>();
    conf.AddConsumer<Order.API.Consumer.Stock.StockNotReservedEventConsumer>();
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host(builder.Configuration["RabbitMQ"]);
        _conf.ReceiveEndpoint(RabbitMQSettings.Order_PaymnetCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));

        _conf.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e => e.ConfigureConsumer<Order.API.Consumer.Stock.StockNotReservedEventConsumer>(context));

        _conf.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
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

app.Run();
