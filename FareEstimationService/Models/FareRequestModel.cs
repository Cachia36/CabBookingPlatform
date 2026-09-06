namespace FareEstimationService.Models
{
    public class FareRequestModel
    {
        public string CabType { get; set; } = string.Empty;
        public int PassengerCount { get; set; }
        public DateTime RideDateTime { get; set; }
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public bool IsDiscountEligible { get; set; }
    }
}