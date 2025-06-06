using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebFrontend.Models
{
    public class Location
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; }
    }
}
