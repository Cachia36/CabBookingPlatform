using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly HttpClient _httpClient;

        public PaymentController(MongoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
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
            return 10.0; //for now
        }
    }
}
