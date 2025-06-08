using Microsoft.AspNetCore.Routing.Constraints;

namespace Shared.Contracts
{
    public class CabReadyEvent
    {
        public string UserId { get; set; }
        public string BookingId { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime DateTime { get; set; }
        public int PassengerCount {  get; set; }
        public string CabType {  get; set; }
        public float TotalPrice { get; set; }
    }
}