using Shared.Contracts;
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
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public BookingController(MongoDbContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                await _context.Bookings.InsertOneAsync(booking);
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:BookingCompleted"));
                await endpoint.Send(new BookingCompletedEvent
                {
                    UserId = booking.UserId,
                    BookingId = booking.Id!,
                    CompletedAt = DateTime.UtcNow
                });

                endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CabReadyEvent"));

                await endpoint.Send(new CabReadyEvent
                {
                    UserId = booking.UserId,
                    BookingId = booking.Id!,
                    PickupLocation = booking.StartLocation,
                    Destination = booking.EndLocation,
                    ScheduledTime = booking.DateTime
                }, context =>
                {
                    context.Delay = TimeSpan.FromMinutes(3); 
                });

                return Ok("Booking created successfully");
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Booking creation failed: {ex.Message}");
            }

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
