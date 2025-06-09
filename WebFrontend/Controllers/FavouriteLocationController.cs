using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class FavouriteLocationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FavouriteLocationController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> FavouriteLocations()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/location/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load Booking Details.";
                return RedirectToAction("Login", "Account");
            }

            var locations = await response.Content.ReadFromJsonAsync<List<Location>>();
            return View(locations);
        }

        [HttpGet]
        public IActionResult AddFavouriteLocation()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFavouriteLocation(Location model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            Console.WriteLine(model);

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/location/add", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Location saved successfully!";
                return RedirectToAction("FavouriteLocations");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Failed to save location: {error}");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditFavouriteLocation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "Invalid location ID.";
                return RedirectToAction("FavouriteLocations");
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/location/{userId}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load locations.";
                return RedirectToAction("FavouriteLocations");
            }

            var location = await response.Content.ReadFromJsonAsync<Location>();

            if (location == null)
            {
                TempData["Message"] = "Location not found.";
                return RedirectToAction("FavouriteLocations");
            }

            return View("EditFavouriteLocation", location);
        }

        [HttpPost]
        public async Task<IActionResult> EditFavouriteLocation(Location model)
        {
            Console.WriteLine(">>>> POST EditFavouriteLocation triggered");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID");

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(model);

            }
            Console.WriteLine($"Sending to API: Id = {model.Id}, City = {model.City}");
            var baseUrl = _config["GatewayService:BaseUrl"];
            var updatePayload = new
            {
                id = model.Id,
                city = model.City
            };

            var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/location/update", updatePayload);
            Console.WriteLine("Updating city for location ID: " + updatePayload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Location updated successfully!";
                return RedirectToAction("FavouriteLocations");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("PUT result: " + content);
            Console.WriteLine("Status Code: " + response.StatusCode);
            ModelState.AddModelError(string.Empty, $"Failed to update location: {content}");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFavouriteLocation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid location ID.";
                return RedirectToAction("FavouriteLocations");
            }

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.DeleteAsync($"{baseUrl}/location/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Location deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete location: {content}";
            }

            return RedirectToAction("FavouriteLocations");
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                TempData["ErrorMessage"] = "City is required to get weather.";
                return RedirectToAction("FavouriteLocations");
            }

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/location/weather/{city}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to retrieve weather information.";
                return RedirectToAction("FavouriteLocations");
            }

            var raw = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(raw);

            var root = doc.RootElement;
            var weatherVm = new WeatherViewModel
            {
                City = root.GetProperty("location").GetProperty("name").GetString(),
                LocalTime = root.GetProperty("location").GetProperty("localtime").GetString(),
                Condition = root.GetProperty("current").GetProperty("condition").GetProperty("text").GetString(),
                IconUrl = "https:" + root.GetProperty("current").GetProperty("condition").GetProperty("icon").GetString(),
                TemperatureC = root.GetProperty("current").GetProperty("temp_c").GetDouble(),
                FeelsLikeC = root.GetProperty("current").GetProperty("feelslike_c").GetDouble(),
                Humidity = root.GetProperty("current").GetProperty("humidity").GetInt32(),
                WindKph = root.GetProperty("current").GetProperty("wind_kph").GetDouble(),
                WindDir = root.GetProperty("current").GetProperty("wind_dir").GetString(),
                UvIndex = root.GetProperty("current").GetProperty("uv").GetDouble()
            };

            ViewBag.Weather = weatherVm;

            var userId = HttpContext.Session.GetString("UserId");
            var locationsResponse = await _httpClient.GetAsync($"{baseUrl}/location/{userId}");
            var locations = await locationsResponse.Content.ReadFromJsonAsync<List<Location>>();

            return View("FavouriteLocations", locations);
        }
    }
}
