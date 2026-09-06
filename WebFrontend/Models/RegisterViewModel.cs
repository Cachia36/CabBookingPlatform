using System.ComponentModel.DataAnnotations;

namespace WebFrontend.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Display(Name = "First name")]
        public string FirstName {  get; set; } = string.Empty;
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
