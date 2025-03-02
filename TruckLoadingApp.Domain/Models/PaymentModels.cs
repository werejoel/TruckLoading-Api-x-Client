using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    public class PaymentRequest
    {
        public long BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public string? RequestedByUserId { get; set; }
    }

    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public PaymentResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
