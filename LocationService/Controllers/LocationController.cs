using LocationService.Data;
using LocationService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http.Headers;

namespace LocationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _config;

        public LocationController(MongoDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            await _context.Locations.InsertOneAsync(location);
            return Ok("Location saved.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] Location location)
        {
            var filter = Builders<Location>.Filter.Eq("_id", ObjectId.Parse(location.Id));
            var result = await _context.Locations.ReplaceOneAsync(filter, location);

            return result.ModifiedCount > 0 ? Ok("Updated.") : NotFound("Not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            var result = await _context.Locations.DeleteOneAsync(l =>l.Id == id);
            return result.DeletedCount > 0 ? Ok("Deleted.") : NotFound("Not found.");
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserLocations(string userId)
        {
            var locations = await _context.Locations.Find(l => l.UserId == userId).ToListAsync();
            return Ok(locations);
        }

        [HttpGet("weather/{city}")]
        public async Task<IActionResult> GetWeatherForecast(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weatherapi-com.p.rapidapi.com/alerts.json?q={city}"),
                Headers =
                {
                    { "x-rapidapi-key", _config["RapidApi:Key"]},
                    { "x-rapidapi-host", "weatherapi-com.p.rapidapi.com" },
                },
            };
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            
            return Ok(body);
        }
    }
}
