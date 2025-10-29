using CustomerService.Consumers;
using CustomerService.Data;
using CustomerService.Models;
using MassTransit;
using Shared.Contracts;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerExamplesFromAssemblyOf<User>();

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingCompletedConsumer>();
    x.AddConsumer<CabReadyConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("cow.rmq2.cloudamqp.com", "yrsxyedd", h =>
        {
            h.Username("yrsxyedd");
            h.Password("AYvwR3QRo1Y9We-3ricXzS0r-4zseAYe");
        });

        cfg.ReceiveEndpoint("BookingCompleted", e =>
        {
            e.ConfigureConsumer<BookingCompletedConsumer>(context);
        });

        cfg.ReceiveEndpoint("CabReadyEvent", e =>
        {
            e.ConfigureConsumer<CabReadyConsumer>(context);
        });

        cfg.UseDelayedMessageScheduler();
    });
});


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
