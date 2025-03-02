using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingConfirmation(Booking booking);
        Task SendStatusUpdate(Booking booking);
    }
}
