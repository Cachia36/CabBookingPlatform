using PaymentService.DTOs;

namespace PaymentService.Services
{
    public interface IBookingClient
    {
        Task<BookingDto?> GetByIdAsync(string id, CancellationToken ct = default);
    }
}
