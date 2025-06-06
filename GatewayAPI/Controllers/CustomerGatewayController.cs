using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/gateway/customer")]
    public class CustomerGatewayController : ControllerBase
    {
        private readonly ProxyService _proxy;
        public CustomerGatewayController(ProxyService proxy)
        {
            _proxy = proxy;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] object payload)
        {
            var response = await _proxy.ForwardAsync("Customer", "api/User/register", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "text/plain");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] object payload)
        {
            var response = await _proxy.ForwardAsync("Customer", "api/User/login", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var response = await _proxy.ForwardAsync("Customer", $"api/User/{userId}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet("{userId}/inbox")]
        public async Task<IActionResult> GetInbox(string userId)
        {
            var response = await _proxy.ForwardAsync("Customer", $"api/User/{userId}/inbox", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }

}
