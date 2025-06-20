using System.ComponentModel.DataAnnotations;

namespace WebFrontend.Models
{
    public class BookingViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Pick up location")]
        public string StartLocation { get; set; }
        [Required]
        [Display(Name = "Drop off location")]
        public string EndLocation { get; set; }
        [Required(ErrorMessage = "Date and Time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime RideDateTime {  get; set; }
        [Required(ErrorMessage = "Number of passengers is required.")]
        [Range(1, 8, ErrorMessage = "Passengers must be between 1 and 8.")]
        public int PassengerCount {  get; set; }
        [Required]
        public string CabType {  get; set; }
        [Required]
        public float BaseFarePrice {  get; set; }
        [Required]
        public float TotalPrice {  get; set; }
    }
}