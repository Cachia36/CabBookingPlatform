using Microsoft.AspNetCore.Mvc;
using CustomerService.Data;
using CustomerService.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity.Data;

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
            var exists = await _context.Users.Find(u => u.Email == user.Email).AnyAsync();
            if (exists) return BadRequest("User already exists");
            await _context.Users.InsertOneAsync(user);
            return Ok("Register successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest login)
        {
            var user = await _context.Users.Find(u => u.Email == login.Email && u.Password == login.Password).FirstOrDefaultAsync();
            if (user == null) return Unauthorized(new { error = "Invalid email or password" });
            return Ok(new { user.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("{id}/inbox")]
        public async Task<IActionResult> GetInbox(string id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return Ok(user?.Inbox ?? new List<string>());
        }
    }
}