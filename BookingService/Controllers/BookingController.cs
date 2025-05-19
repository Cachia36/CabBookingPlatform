using BookingService.Contarcts;
using BookingService.Data;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MassTransit;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public BookingController(MongoDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            await _context.Bookings.InsertOneAsync(booking);

            await _publishEndpoint.Publish(new BookingCompletedEvent
            {
                UserId = booking.UserId,
                BookingId = booking.Id!,
                CompletedAt = DateTime.UtcNow
            });

            return Ok("Booking created successfully");
        }
        [HttpGet("current/{userId}")]
        public async Task<IActionResult> GetCurrentBookings(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _context.Bookings
                .Find(b => b.UserId == userId && b.DateTime >= now)
                .ToListAsync();
            return Ok(bookings);
        }
        [HttpGet("past/{userId}")]
        public async Task<IActionResult> GetPastBookings(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _context.Bookings
                .Find(b => b.UserId == userId && b.DateTime < now)
                .ToListAsync();

            return Ok(bookings);
        }
    }
}
