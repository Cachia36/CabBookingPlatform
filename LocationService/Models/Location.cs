using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationService.Models
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; } 
        public string City { get; set; } 
    }
}
