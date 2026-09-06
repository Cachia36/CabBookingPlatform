namespace Shared.Contracts
{
    public class CabReadyEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int PassengerCount { get; set; }
        public string CabType { get; set; } = string.Empty;
        public float TotalPrice { get; set; }
    }
}