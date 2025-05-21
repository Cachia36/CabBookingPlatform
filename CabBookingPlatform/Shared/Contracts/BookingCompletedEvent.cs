namespace Shared.Contracts
{
    public class BookingCompletedEvent
    {
        public string UserId { get; set; }
        public string BookingId { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}