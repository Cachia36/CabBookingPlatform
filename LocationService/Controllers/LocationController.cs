using LocationService.Data;
using LocationService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Http;
using System.Text.Json;

namespace LocationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public LocationController(MongoDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            await _context.Locations.InsertOneAsync(location);
            return Ok("Location saved.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateCityRequest request)
        {
            if (string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.City))
                return BadRequest("Id and City are required.");

            var filter = Builders<Location>.Filter.Eq(l => l.Id, request.Id);
            var update = Builders<Location>.Update.Set(l => l.City, request.City);

            var result = await _context.Locations.UpdateOneAsync(filter, update);

            return result.MatchedCount == 0 ? NotFound("Location not found.") : Ok("City updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            var result = await _context.Locations.DeleteOneAsync(l => l.Id == id);
            return result.DeletedCount > 0 ? Ok("Deleted.") : NotFound("Not found.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserLocations(string userId)
        {
            var locations = await _context.Locations.Find(l => l.UserId == userId).ToListAsync();
            return Ok(locations);
        }

        [HttpGet("{userId}/{locationId}")]
        public async Task<IActionResult> GetLocationById(string userId, string locationId)
        {
            var filter = Builders<Location>.Filter.Eq(l => l.UserId, userId) &
                         Builders<Location>.Filter.Eq(l => l.Id, locationId);

            var location = await _context.Locations.Find(filter).FirstOrDefaultAsync();

            return location == null ? NotFound("Location not found.") : Ok(location);
        }

        [HttpGet("weather/{city}")]
        public async Task<IActionResult> GetWeatherForecast(string city)
        {
            var (lat, lng) = await GetCoordinatesAsync(city);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weatherapi-com.p.rapidapi.com/current.json?q={lat},{lng}"),
                Headers =
                {
                    { "x-rapidapi-key", _config["RapidApi:Key"] },
                    { "x-rapidapi-host", "weatherapi-com.p.rapidapi.com" },
                },
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            return Ok(body);
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
