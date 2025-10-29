using PaymentService.DTOs;

namespace PaymentService.Services
{
    public class BookingClient : IBookingClient
    {
        private readonly HttpClient _http;
        public BookingClient(HttpClient http) => _http = http;

        public async Task<BookingDto?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var resp = await _http.GetAsync($"/api/gateway/booking/{id}", ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<BookingDto>(cancellationToken: ct);
        }
    }
}
