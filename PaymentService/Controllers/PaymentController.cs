using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PaymentService.Data;
using PaymentService.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public PaymentController(MongoDbContext context, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] FareRequest request)
        {
            if (request.PassengerCount > 8)
                return BadRequest("Max passengers is 8");

            double cabMultiplier = request.CabType switch
            {
                "Premium" => 1.2,
                "Executive" => 1.4,
                _ => 1.0
            };

            double timeMultiplier = (request.RideDateTime.Hour >= 0 && request.RideDateTime.Hour < 8) ? 1.2 : 1.0;

            double passengersMultiplier = (request.PassengerCount <= 4) ? 1 :
                                          (request.PassengerCount <= 8) ? 2 : 0;

            double discountMultiplier = request.IsDiscountEligible ? 0.9 : 1.0;

            double baseFare = await GetBaseFareAsync(request.StartLocation, request.EndLocation);
            double totalPrice = baseFare * cabMultiplier * timeMultiplier * passengersMultiplier * discountMultiplier;

            var payment = new Payment
            {
                UserId = request.UserId,
                BookingId = request.BookingId,
                BaseFare = baseFare,
                TotalPrice = totalPrice,
            };

            await _context.Payments.InsertOneAsync(payment);

            return Ok(payment);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPayments(string userId)
        {
            var payments = await _context.Payments.Find(p => p.UserId == userId).ToListAsync();
            return Ok(payments);
        }
        private async Task<double> GetBaseFareAsync(string start, string end)
        {
            //1. convert start and end into lat/lng coordinates
            var (depLat, depLng) = await GetCoordinatesAsync(start);
            var (arrLat, arrLng) = await GetCoordinatesAsync(end);

            //2. Call FareEstimationService
            var url = $"https://localhost:7186/api/fareestimation?depLat={depLat}&depLng={depLng}&arrLat={arrLat}&arrLng={arrLng}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("FareEstimationService error response:");
                Console.WriteLine(error); 
                throw new Exception("Failed to retrieve fare from FareEstimationService");
            }


            var json = await response.Content.ReadFromJsonAsync<FareEstimateResult>();
            return (double)(json?.EstimatedFare ?? 0);
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

            var firstResult = doc.RootElement
                                 .GetProperty("results")[0]
                                 .GetProperty("geometry");

            double lat = firstResult.GetProperty("lat").GetDouble();
            double lng = firstResult.GetProperty("lng").GetDouble();

            return (lat, lng);
        }

    }
}