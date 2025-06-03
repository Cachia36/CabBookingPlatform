using FareEstimationService.Models;
using System.Text.Json;

namespace FareEstimationService.Services
{
    public class FareEstimationService
    {
        private readonly IConfiguration _configuration;

        public FareEstimationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<FareEstimateResult?> GetFareEstimateAsync(double depLat, double depLng, double arrLat, double arrLng)
        {
            var url = $"https://taxi-fare-calculator.p.rapidapi.com/search-geo?dep_lat={depLat}&dep_lng={depLng}&arr_lat={arrLat}&arr_lng={arrLng}";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", _configuration["RapidApi:Key"] },
                    { "x-rapidapi-host", "taxi-fare-calculator.p.rapidapi.com" }
                },
            };

            using var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            if (!json.TryGetProperty("journey", out var journey) ||
                !journey.TryGetProperty("fares", out var fares) ||
                fares.GetArrayLength() == 0)
            {
                return null;
            }

            var priceProp = fares[0].GetProperty("price_in_cents");
            decimal estimatedFare = 0;

            if (priceProp.ValueKind == JsonValueKind.Number && priceProp.TryGetInt32(out var cents))
            {
                estimatedFare = cents / 100m;
            }
            else
            {
                Console.WriteLine("Warning: price_in_cents is not a number.");
                return null;
            }

            int duration = 0;
            if (journey.TryGetProperty("duration", out var durationProp) && durationProp.TryGetInt32(out var d))
                duration = d;

            int distance = 0;
            if (journey.TryGetProperty("distance", out var distanceProp) && distanceProp.TryGetInt32(out var dist))
                distance = dist;

            return new FareEstimateResult
            {
                EstimatedFare = estimatedFare,
                Duration = $"{duration} mins",
                Distance = $"{distance} km"
            };
        }
    }
}
