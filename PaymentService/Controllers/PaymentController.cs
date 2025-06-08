using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using MongoDB.Driver;
using PaymentService.Data;
using PaymentService.Models;
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
        public PaymentController(MongoDbContext context, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentModel model)
        {
            
            var baseUrl = _config["GatewayService:BaseUrl"];
            var payment = new PaymentModel
            {
                UserId = model.UserId,
                BookingId = model.BookingId,
                TotalPrice = model.TotalPrice
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
    }
}