using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBooking(Load load, Truck truck, decimal agreedPrice, string priceCalculationMethod, string currency);
        Task<Booking?> GetBookingById(long bookingId);
        Task<bool> UpdateBookingStatus(long bookingId, BookingStatusEnum newStatus);
    }
}
