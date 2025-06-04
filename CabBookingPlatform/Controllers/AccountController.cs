using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AccountController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _config = config;
        }
        private async Task<bool> SignUserInAsync(string email, string password)
        {
            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/customer/login", new LoginViewModel { Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
                return false;

            var rawJson = await response.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<Dictionary<string, string>>(rawJson);
            var userId = loginResult?["id"];

            if (string.IsNullOrEmpty(userId))
                return false;

            HttpContext.Session.SetString("UserId", userId);
            return true;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Console.WriteLine("Logging in");
            var success = await SignUserInAsync(model.Email, model.Password);

            if (success)
            {
                Console.WriteLine("Logged in, should redirect");
                Console.WriteLine(HttpContext.Session.GetString("UserId"));
                return RedirectToAction("ViewAccount", "Account");
            }

            ModelState.AddModelError(string.Empty, "Login failed");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/customer/register", model);

            if (response.IsSuccessStatusCode)
            {
                var loginSuccess = await SignUserInAsync(model.Email, model.Password);
                if (loginSuccess)
                    return RedirectToAction("ViewAccount", "Account");

                ModelState.AddModelError(string.Empty, "Registration succeeded but login failed.");
                return View(model);
            }
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Registration failed: {error}");
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            Console.WriteLine(userId);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/customer/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load account details.";
                Console.WriteLine("Failed to load account details");
                return RedirectToAction("Login");
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/customer/{userId}/inbox");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load inbox";
                return RedirectToAction("Login");
            }

            var messages = await response.Content.ReadFromJsonAsync<List<string>>();
            var model = new InboxViewModel { Messages = messages };
            return View(model);
        }
    }
}