using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentService.Models
{
    public class PaymentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; } 
        public string BookingId { get; set; } 
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
