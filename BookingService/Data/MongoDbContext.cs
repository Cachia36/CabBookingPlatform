using BookingService.Models;
using MongoDB.Driver;

namespace BookingService.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("CabBookingDb_Booking");
    }

    public IMongoCollection<Booking> Bookings => _database.GetCollection<Booking>("Bookings");
}
