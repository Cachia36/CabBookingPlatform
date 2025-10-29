using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using MongoDB.Driver;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.Services;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IBookingClient _bookingClient;

        public PaymentController(MongoDbContext context, IHttpClientFactory httpClientFactory, IConfiguration config, IBookingClient bookingClient)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
            _bookingClient = bookingClient;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentModel model, CancellationToken ct)
        {
            var booking = await _bookingClient.GetByIdAsync(model.BookingId, ct);
            if (booking is null) return NotFound("Booking not found");

            if (model.TotalPrice != booking.TotalPrice)
                return BadRequest("Invalid payment amount");

            var payment = new PaymentModel
            {
                UserId = model.UserId,
                BookingId = model.BookingId,
                TotalPrice = booking.TotalPrice
            };

            await _context.Payments.InsertOneAsync(payment, cancellationToken: ct);
            return Ok(payment);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPayments(string userId)
        {
            var payments = await _context.Payments.Find(p => p.UserId == userId).ToListAsync();
            return Ok(payments);
        }
    }
}