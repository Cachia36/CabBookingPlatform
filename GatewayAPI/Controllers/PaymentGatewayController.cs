using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/gateway/payment")]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly ProxyService _proxy;
        public PaymentGatewayController(ProxyService proxy)
        {
            _proxy = proxy;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] object payload)
        {
            var response = await _proxy.ForwardAsync("Payment", "api/Payment/pay", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPaymentHistory(string userId)
        {
            var response = await _proxy.ForwardAsync("Payment", $"api/Payment/{userId}", HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
