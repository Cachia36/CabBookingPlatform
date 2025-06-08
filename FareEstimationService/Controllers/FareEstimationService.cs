using Microsoft.AspNetCore.Mvc;
using FareEstimationService.Models;
using System.Net.Http;
using System.Text.Json;

namespace FareEstimationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FareEstimationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly FareEstimationService.Services.FareEstimationService _fareService;

        public FareEstimationController(IHttpClientFactory httpClientFactory, IConfiguration config, FareEstimationService.Services.FareEstimationService fareService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
            _fareService = fareService;
        }

        [HttpPost("estimate")]
        public async Task<IActionResult> EstimateFare([FromBody] FareRequestModel request)
        {
            try
            {
                if (request.PassengerCount > 8)
                    return BadRequest("Max passengers is 8");

                double cabMultiplier = request.CabType switch
                {
                    "Premium" => 1.2,
                    "Executive" => 1.4,
                    _ => 1.0
                };

                double timeMultiplier = request.RideDateTime.Hour < 8 ? 1.2 : 1.0;
                double passengersMultiplier = request.PassengerCount <= 4 ? 1 : 2;
                double discountMultiplier = request.IsDiscountEligible ? 0.9 : 1.0;

                // Get coordinates
                var (depLat, depLng) = await GetCoordinatesAsync(request.StartLocation);
                var (arrLat, arrLng) = await GetCoordinatesAsync(request.EndLocation);

                // Call internal service for base fare (from RapidAPI)
                var baseFare = await _fareService.GetFareEstimateAsync(depLat, depLng, arrLat, arrLng);
                if (baseFare == null)
                    return StatusCode(502, "Failed to retrieve base fare from RapidAPI.");

                double totalPrice = (double)baseFare * cabMultiplier * timeMultiplier * passengersMultiplier * discountMultiplier;

                return Ok(new { BaseFarePrice = baseFare, TotalPrice = totalPrice });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<(double lat, double lng)> GetCoordinatesAsync(string location)
        {
            string encodedLocation = Uri.EscapeDataString(location);
            string apiKey = _config["OpenCage:Key"];
            string url = $"https://api.opencagedata.com/geocode/v1/json?q={encodedLocation}&key={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to convert address to coordinates");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var geometry = doc.RootElement.GetProperty("results")[0].GetProperty("geometry");

            return (geometry.GetProperty("lat").GetDouble(), geometry.GetProperty("lng").GetDouble());
        }
    }
}
