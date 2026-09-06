namespace WebFrontend.Models
{
    public class Payment
    {
        public string UserId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public float TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
