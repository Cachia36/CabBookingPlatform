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

        public async Task<decimal?> GetFareEstimateAsync(double depLat, double depLng, double arrLat, double arrLng)
        {
            var url = $"https://taxi-fare-calculator.p.rapidapi.com/search-geo?dep_lat={depLat}&dep_lng={depLng}&arr_lat={arrLat}&arr_lng={arrLng}";

            using var client = new HttpClient();
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
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("journey", out var journey) ||
                !journey.TryGetProperty("fares", out var fares) ||
                fares.GetArrayLength() == 0)
            {
                return null;
            }

            var priceProp = fares[0].GetProperty("price_in_cents");

            if (priceProp.ValueKind == JsonValueKind.Number && priceProp.TryGetInt32(out int cents))
            {
                return cents / 100m;
            }

            Console.WriteLine("Warning: 'price_in_cents' is not a valid number.");
            return null;
        }
    }
}
