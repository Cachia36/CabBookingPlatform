using Microsoft.AspNetCore.Mvc;
using FareEstimationService.Services;
using FareEstimationService.Models;

namespace FareEstimationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FareEstimationController : ControllerBase
    {
        private readonly Services.FareEstimationService _fareService;

        public FareEstimationController(Services.FareEstimationService fareService)
        {
            _fareService = fareService;
        }

        [HttpGet]
        public async Task<ActionResult<FareEstimateResult>> Get([FromQuery] double depLat, [FromQuery] double depLng,
                                                                [FromQuery] double arrLat, [FromQuery] double arrLng)
        {
            var result = await _fareService.GetFareEstimateAsync(depLat, depLng, arrLat, arrLng);
            if (result == null) return StatusCode(502, "Failed to retrieve fare estimate.");

            return Ok(result);
        }
    }
}
