using MongoDB.Driver;
using PaymentService.Models;

namespace PaymentService.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("CabBookingDb_Payment");
        }
        public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("Payments");
    }
}
