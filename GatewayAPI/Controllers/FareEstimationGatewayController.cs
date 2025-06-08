using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/gateway/fareEstimation")]
    public class FareEstimationGatewayController : ControllerBase
    {
        private readonly ProxyService _proxy;
        public FareEstimationGatewayController(ProxyService proxy)
        {
            _proxy = proxy;
        }
        [HttpPost("estimate")]
        public async Task<IActionResult> EstimateFare([FromBody] object payload)
        {
            var response = await _proxy.ForwardAsync("FareEstimation", "api/FareEstimation/estimate", HttpMethod.Post, payload);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
