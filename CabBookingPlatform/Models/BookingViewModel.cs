using System.ComponentModel.DataAnnotations;

namespace CabBookingPlatform.Models
{
    public class BookingViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string StartLocation { get; set; }
        [Required]
        public string EndLocation { get; set; }
        [Required]
        public DateTime DateTime {  get; set; }
        [Required]
        public int PassengerCount {  get; set; }
        [Required]
        public string CabType {  get; set; }
    }
}