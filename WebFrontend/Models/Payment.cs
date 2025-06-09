namespace WebFrontend.Models
{
    public class Payment
    {
        public string UserId {  get; set; }
        public string BookingId {  get; set; }
        public float TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
