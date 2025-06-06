using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebFrontend.Models
{
    public class WeatherViewModel
    {
        public string City { get; set; }
        public string LocalTime { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
        public double TemperatureC { get; set; }
        public double FeelsLikeC { get; set; }
        public int Humidity { get; set; }
        public double WindKph { get; set; }
        public string WindDir { get; set; }
        public double UvIndex { get; set; }
    }
}
