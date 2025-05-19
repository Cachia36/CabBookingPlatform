namespace CustomerService.Contracts
{
    public class BookingCompletedEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
    }
}
