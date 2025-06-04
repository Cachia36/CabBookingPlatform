using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

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

        
    }
}
