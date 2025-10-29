using CustomerService.Models;
using MongoDB.Driver;

namespace CustomerService.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("CabBookingDb_User");
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}
