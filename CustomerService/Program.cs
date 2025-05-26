using Shared.Contracts;
using CustomerService.Data;
using MassTransit;
using CustomerService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingCompletedConsumer>();
    x.AddConsumer<CabReadyConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
