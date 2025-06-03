namespace PaymentService.Models
{
    public class FareEstimateResult
    {
        public decimal EstimatedFare { get; set; }
        public string Currency { get; set; }
        public string Duration { get; set; }
        public string Distance {  get; set; }
    }
}
