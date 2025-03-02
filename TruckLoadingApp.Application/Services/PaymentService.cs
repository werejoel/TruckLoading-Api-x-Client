using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest request)
        {
            _logger.LogInformation($"Processing payment for BookingId: {request.BookingId}, Amount: {request.Amount}, Requested by: {request.RequestedByUserId ?? "Unknown"}");

            var booking = await _context.Bookings.FindAsync(request.BookingId);
            if (booking == null)
                return new PaymentResponse(false, "Booking not found");

            var payment = new PaymentDetails
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatusEnum.Pending,
                CreatedDate = DateTime.UtcNow
            };

            _context.PaymentDetails.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Payment recorded for BookingId: {request.BookingId}, PaymentId: {payment.Id}, Requested by: {request.RequestedByUserId ?? "Unknown"}");
            return new PaymentResponse(true, "Payment recorded successfully");
        }

        public async Task<PaymentDetails?> GetPaymentById(long paymentId)
        {
            return await _context.PaymentDetails.FindAsync(paymentId);
        }
    }
}
