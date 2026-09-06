using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentService.Models
{
    public class PaymentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}