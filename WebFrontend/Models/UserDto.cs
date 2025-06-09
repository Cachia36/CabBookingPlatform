using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebFrontend.Models
{
    public class UserDto
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Inbox { get; set; }
        public int BookingCount { get; set; }
        public bool HasReceivedDiscount { get; set; }
    }

}
