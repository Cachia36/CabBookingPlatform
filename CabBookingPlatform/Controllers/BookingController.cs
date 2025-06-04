using CabBookingPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public BookingController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _config = config;
        }

        [HttpGet]
        public IActionResult CreateBooking()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = _config["BookingService:BaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/Booking/create", model);

            Console.WriteLine("CreateBooking called");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Booking created successfully!";
                return RedirectToAction("ViewAccount", "Account");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Booking failed: {error}");
            return View(model);
        }
    }
}
