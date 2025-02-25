using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPayment(PaymentRequest request);
        Task<PaymentDetails?> GetPaymentById(long paymentId);
    }
}
