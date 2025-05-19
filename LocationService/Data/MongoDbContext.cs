using LocationService.Models;
using MongoDB.Driver;

namespace LocationService.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("CabBookingDb_Location");
    }

    public IMongoCollection<Location> Locations => _database.GetCollection<Location>("Locations");
}
