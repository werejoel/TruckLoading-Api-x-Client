using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingConfirmation(Booking booking);
        Task SendStatusUpdate(Booking booking);
    }
}
