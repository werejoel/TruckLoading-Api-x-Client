using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Booking> CreateBooking(Load load, Truck truck, decimal agreedPrice, string priceCalculationMethod, string currency)
        {
            _logger.LogInformation($"Creating booking for LoadId: {load.Id}, TruckId: {truck.Id}");
            var booking = new Booking
            {
                LoadId = load.Id,
                TruckId = truck.Id,
                AgreedPrice = agreedPrice,
                PriceCalculationMethod = priceCalculationMethod,
                Currency = currency
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Booking created successfully with BookingId: {booking.Id}");
            return booking;
        }
        public async Task<Booking?> GetBookingById(long bookingId)
        {
            _logger.LogInformation($"Retrieving booking for BookingId: {bookingId}");
            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task<bool> UpdateBookingStatus(long bookingId, BookingStatusEnum newStatus)
        {
            _logger.LogInformation($"Updating booking status for BookingId: {bookingId} to {newStatus}");

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with BookingId: {bookingId} not found");
                return false;
            }

            booking.Status = newStatus;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Booking status updated successfully for BookingId: {bookingId}");
            return true;
        }
    }
}
