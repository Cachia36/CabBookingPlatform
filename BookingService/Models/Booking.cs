using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime RideDateTime { get; set; }
        public int PassengerCount { get; set; }
        public string CabType { get; set; } = string.Empty;
        public float BaseFarePrice { get; set; }
        public float TotalPrice {  get; set; }
    }
}
