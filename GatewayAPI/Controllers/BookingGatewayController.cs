using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/gateway/booking")]
    public class BookingGatewayController : ControllerBase
    {
        private readonly ProxyService _proxy;
        public BookingGatewayController(ProxyService proxy)
        {
            _proxy = proxy;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] object payload)
        {
            var response = await _proxy.ForwardAsync("Booking", "api/Booking/create", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "text/plain");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(string id)
        {
            var response = await _proxy.ForwardAsync("Booking", $"api/Booking/{id}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = content,
                ContentType = contentType
            };
        }

        [HttpGet("current/{userId}")]
        public async Task<IActionResult> GetCurrentBookings(string userId)
        {
            var response = await _proxy.ForwardAsync("Booking", $"api/Booking/current/{userId}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet("past/{userId}")]
        public async Task<IActionResult> GetPastBookings(string userId)
        {
            var response = await _proxy.ForwardAsync("Booking", $"api/Booking/past/{userId}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
