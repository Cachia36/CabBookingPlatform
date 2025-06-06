using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/gateway/location")]
    public class LocationGatewayController : ControllerBase
    {
        private readonly ProxyService _proxy;
        public LocationGatewayController(ProxyService proxy)
        {
            _proxy = proxy;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddLocation([FromBody] object payload)
        {
            Console.WriteLine("Adding Location..");
            var response = await _proxy.ForwardAsync("Location", "api/Location/add", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "text/plain");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] object payload)
        {
            Console.WriteLine("Calling API");
            var response = await _proxy.ForwardAsync("Location", "api/Location/update", HttpMethod.Put, payload);
            var content = await response.Content.ReadAsStringAsync();

            // Optional: inspect response content-type if needed
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "text/plain";
            return Content(content, contentType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            var response = await _proxy.ForwardAsync("Location", $"api/Location/{id}", HttpMethod.Delete);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "text/plain");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserLocations(string userId)
        {
            var response = await _proxy.ForwardAsync("Location", $"api/Location/{userId}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet("{userId}/{locationId}")]
        public async Task<IActionResult> GetLocationById(string userId, string locationId)
        {
            var response = await _proxy.ForwardAsync(
                "Location",
                $"api/Location/{userId}/{locationId}",
                HttpMethod.Get
            );

            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
            return Content(content, contentType);
        }

        [HttpGet("weather/{city}")]
        public async Task<IActionResult> GetWeatherForecast(string city)
        {
            var response = await _proxy.ForwardAsync("Location", $"api/Location/weather/{city}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
