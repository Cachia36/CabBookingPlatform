namespace FareEstimationService.Models
{
    public class FareRequestModel
    {
        public string CabType {  get; set; } //Economic, Premium, Executive
        public int PassengerCount { get; set; }
        public DateTime RideDateTime { get; set; } 
        public string StartLocation {  get; set; }
        public string EndLocation { get; set; }
        public bool IsDiscountEligible {  get; set; }
    }
}
