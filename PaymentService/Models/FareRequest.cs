namespace PaymentService.Models
{
    public class FareRequest
    {
        public string UserId { get; set; }
        public string BookingId {  get; set; }
        public string CabType {  get; set; } //Economic, Premium, Executive
        public int PassengerCount { get; set; }
        public DateTime RideDateTime { get; set; }
        public string StartLocation {  get; set; }
        public string EndLocation { get; set; }
        public bool IsDiscountEligible {  get; set; }
    }
}
