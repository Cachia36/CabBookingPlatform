using Microsoft.AspNetCore.Mvc;
using CustomerService.Data;
using CustomerService.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity.Data;
using BCrypt.Net;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MongoDbContext _context;
        public UserController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            //normalize email
            var email = user.Email.Trim().ToLowerInvariant();

            //check if exists
            var exists = await _context.Users.Find(u => u.Email == user.Email).AnyAsync();
            if (exists) return BadRequest("User already exists");

            // hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.Email = email;
            user.Password = hashedPassword;
            user.Inbox = new List<string>();
            user.BookingCount = 0;
            user.HasReceivedDiscount = false;

            await _context.Users.InsertOneAsync(user);
            return Ok("Register successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest login)
        {
            var email = login.Email.Trim().ToLowerInvariant();

            // find user by email only
            var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) return Unauthorized(new { error = "Invalid email or password" });

            // verify password
            bool valid = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
            if (!valid) return Unauthorized(new { error = "Invalid email or password" });

            return Ok(new { user.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.BookingCount,
                user.HasReceivedDiscount
            });
        }

        [HttpGet("{id}/inbox")]
        public async Task<IActionResult> GetInbox(string id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return Ok(user?.Inbox ?? new List<string>());
        }
    }
}