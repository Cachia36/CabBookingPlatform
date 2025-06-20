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
        private readonly ILogger<BookingController> _logger;

        public BookingController(IHttpClientFactory factory, IConfiguration config, ILogger<BookingController> logger)
        {
            _httpClient = factory.CreateClient();
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreateBooking()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            //Retrieve isDiscountEligible
            var baseUrl = _config["GatewayService:BaseUrl"];

            var userResponse = await _httpClient.GetAsync($"{baseUrl}/customer/{userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var json = await userResponse.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user != null)
                {
                    _logger.LogInformation($"HasReceivedDiscount: {user.HasReceivedDiscount}");
                    bool HasReceivedDiscount = user.HasReceivedDiscount;
                    ViewBag.HasReceivedDiscount = HasReceivedDiscount;
                }
            }
            else
            {
                var errorDetails = await userResponse.Content.ReadAsStringAsync();
                _logger.LogError("Failed to retrieve user info from {Url}. StatusCode: {Code}, Body: {Body}",
                    $"{baseUrl}/customer/{userId}", userResponse.StatusCode, errorDetails);

                ViewBag.HasReceivedDiscount = false;
            }

            //Retrieve Saved Locations
            ViewBag.GatewayBaseUrl = baseUrl;

            var SavedLocationsResponse = await _httpClient.GetAsync($"{baseUrl}/Location/{userId}");

            if (SavedLocationsResponse.IsSuccessStatusCode)
            {
                var json = await SavedLocationsResponse.Content.ReadAsStringAsync();
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
        public async Task<IActionResult> CreateBooking([FromForm] BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }

                var userId = HttpContext.Session.GetString("UserId");
                await PopulateSavedLocations(userId);
                return View(model);
            }

            var baseUrl = _config["GatewayService:BaseUrl"];
            model.UserId = HttpContext.Session.GetString("UserId");

            var bookingResponse = await _httpClient.PostAsJsonAsync($"{baseUrl}/booking/create", model);

            if (!bookingResponse.IsSuccessStatusCode)
            {
                var error = await bookingResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Booking failed: {error}");
                Console.WriteLine("Booking failed");
                return View(model);
            }

            var bookingResult = await bookingResponse.Content.ReadFromJsonAsync<BookingResponseModel>();

            if (bookingResult == null || string.IsNullOrEmpty(bookingResult.BookingId))
            {
                TempData["Message"] = "Booking was created but no ID was returned.";
                Console.WriteLine("Booking was created but no ID was returned");
                return RedirectToAction("Bookings", "Booking");
            }

            var payment = new Payment
            {
                UserId = model.UserId,
                BookingId = bookingResult.BookingId,
                TotalPrice = model.TotalPrice
            };

            bool paymentSuccess = await CreatePaymentAsync(payment);

            if (!paymentSuccess)
            {
                TempData["Message"] = "Booking succeeded, but payment failed.";
                return RedirectToAction("Bookings", "Booking");
            }

            TempData["SuccessMessage"] = "Booking and payment completed successfully!";
            return RedirectToAction("Bookings", "Booking");

        }

        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            var baseUrl = _config["GatewayService:BaseUrl"];

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/Payment/pay", payment);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Payment failed: " + await response.Content.ReadAsStringAsync());
                return false;
            }

            return true;
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