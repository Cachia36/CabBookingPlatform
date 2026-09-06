using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebFrontend.Models
{
    public class WeatherViewModel
    {
        public string City { get; set; } = string.Empty;
        public string LocalTime { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string WindDir { get; set; } = string.Empty;
        public double TemperatureC { get; set; }
        public double FeelsLikeC { get; set; }
        public int Humidity { get; set; }
        public double WindKph { get; set; }
        public double UvIndex { get; set; }
    }
}
