using Shared.Contracts;
using BookingService.Data;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MassTransit;
using System.Runtime.InteropServices;
using MassTransit.Transports;
using MongoDB.Bson;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public BookingController(MongoDbContext context, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                Console.WriteLine("Create Booking API Called");

                await _context.Bookings.InsertOneAsync(booking);

                await _publishEndpoint.Publish(new BookingCompletedEvent
                {
                    UserId = booking.UserId,
                    BookingId = booking.Id!,
                    CompletedAt = DateTime.UtcNow
                });

                // Delay task unchanged
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));

                    var cabReadyEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:CabReadyEvent"));
                    await cabReadyEndpoint.Send(new CabReadyEvent
                    {
                        UserId = booking.UserId,
                        BookingId = booking.Id!,
                        StartLocation = booking.StartLocation,
                        EndLocation = booking.EndLocation,
                        DateTime = booking.RideDateTime,
                        PassengerCount = booking.PassengerCount,
                        CabType = booking.CabType,
                        TotalPrice = booking.TotalPrice
                    });

                    Console.WriteLine("CabReadyEvent sent after delay");
                });

                return Ok(new { bookingId = booking.Id, message = "Booking created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Booking creation failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(string id)
        {
            var booking = await _context.Bookings
                .Find(b => b.Id == id)
                .FirstOrDefaultAsync();

            if (booking is not null)
                return Ok(booking);

            if (ObjectId.TryParse(id, out var oid))
            {
                var filter = Builders<Booking>.Filter.Eq("_id", oid);
                booking = await _context.Bookings.Find(filter).FirstOrDefaultAsync();

                if (booking is not null)
                    return Ok(booking);
            }

            return NotFound(new { message = "Booking not found.", id});
        }

        [HttpGet("current/{userId}")]
        public async Task<IActionResult> GetCurrentBookings(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _context.Bookings
                .Find(b => b.UserId == userId && b.RideDateTime >= now)
                .ToListAsync();
            return Ok(bookings);
        }

        [HttpGet("past/{userId}")]
        public async Task<IActionResult> GetPastBookings(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _context.Bookings
                .Find(b => b.UserId == userId && b.RideDateTime < now)
                .ToListAsync();

            return Ok(bookings);
        }
    }
}
