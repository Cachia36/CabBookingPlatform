using BookingService.Data;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly MongoDbContext _context;
        public BookingController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            await _context.Bookings.InsertOneAsync(booking);
            return Ok("Booking created successfully");
        }
        [HttpPost("current/{userId}")]
        public async Task<IActionResult> GetCurrentBookings(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _context.Bookings
                .Find(b => b.UserId == userId && b.DateTime >= now)
                .ToListAsync();
            return Ok(bookings);
        }
        [HttpPost("past/{userId}")]
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
