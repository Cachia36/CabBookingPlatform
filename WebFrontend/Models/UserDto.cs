using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebFrontend.Models
{
    public class UserDto
    {
        public string id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Inbox { get; set; } = new();
        public int BookingCount { get; set; }
        public bool HasReceivedDiscount { get; set; }
    }

}
