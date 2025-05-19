using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentService.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public double BaseFare { get; set; }
        public double TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
