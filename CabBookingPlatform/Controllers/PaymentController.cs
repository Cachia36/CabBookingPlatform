using Microsoft.AspNetCore.Mvc;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public PaymentController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var baseUrl = _config["GatewayService:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/payment/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Failed to load payments.";
                return RedirectToAction("Index", "Home");
            }

            var payments = await response.Content.ReadFromJsonAsync<List<Payment>>();
            return View(payments);
        }
    }
}
