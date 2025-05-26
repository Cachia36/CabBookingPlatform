namespace Shared.Contracts
{
    public class CabReadyEvent
    {
        public string UserId { get; set; }
        public string BookingId { get; set; }
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}