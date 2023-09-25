using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Models.Entities;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<OrderCreatedEventConsumer>();
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host(builder.Configuration["RabbitMQ"]);
        _conf.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();


#region SeedData

using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();

MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();

var collection = mongoDBService.GetCollection<Stock.API.Models.Entities.Stock>();

if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { ProductId = Guid.Parse("19cf3e11-3353-41cb-8ecb-03e996cd0a38"), Count = 201 });
    await collection.InsertOneAsync(new() { ProductId =  Guid.Parse("887b8fad-e60e-44c3-a5fb-6e7142830e71"), Count = 3333 });
    await collection.InsertOneAsync(new() { ProductId =  Guid.Parse("73bfba9e-40f8-430b-ad90-b98d5e75c77c"), Count = 2222 });
    await collection.InsertOneAsync(new() { ProductId = Guid.Parse("de4592be-cce5-4aff-a2f7-5497b2065087"), Count = 1111 });
}

#endregion
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