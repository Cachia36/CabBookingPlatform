using WebFrontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

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
        public async Task<IActionResult> CreateBooking()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            //Retrieve Saved Locations
            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/Location/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var locationObjects = JsonSerializer.Deserialize<List<Location>>(json);
                var cityNames = locationObjects?.Select(l => l.City).ToList();
                ViewBag.SavedLocations = cityNames ?? new List<string>();
            }
            else
            {
                TempData["Message"] = "Failed to load saved locations.";
                ViewBag.SavedLocations = new List<string>();
            }

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await PopulateSavedLocations(userId);
                return View(model);
            }

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/Booking/create", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Booking created successfully!";
                return RedirectToAction("Bookings", "Booking");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Booking failed: {error}");
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/booking/current/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load Booking Details.";
                return RedirectToAction("Login", "Account");
            }

            var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
            return View(bookings); 
        }

        [HttpGet]
        public async Task<IActionResult> BookingHistory()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/booking/past/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load Booking Details.";
                return RedirectToAction("Login", "Account");
            }

            var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
            return View(bookings);
        }
        private async Task PopulateSavedLocations(string userId)
        {
            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/Location/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var locationObjects = JsonSerializer.Deserialize<List<Location>>(json);
                var cityNames = locationObjects?.Select(l => l.City).ToList();
                ViewBag.SavedLocations = cityNames ?? new List<string>();
                Console.WriteLine(JsonSerializer.Serialize(locationObjects));
            }
            else
            {
                ViewBag.SavedLocations = new List<string>();
            }
        }

    }
}
