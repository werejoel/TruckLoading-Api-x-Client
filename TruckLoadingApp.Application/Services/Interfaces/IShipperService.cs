using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IShipperService
    {
        // Load management
        Task<Load> CreateLoadAsync(Load load, string shipperId);
        Task<IEnumerable<Load>> GetShipperLoadsAsync(string shipperId);
        Task<Load?> GetShipperLoadByIdAsync(string shipperId, long loadId);
        Task<bool> UpdateLoadAsync(Load load, string shipperId);
        Task<bool> DeleteLoadAsync(long loadId, string shipperId);
        
        // Truck search
        Task<IEnumerable<Truck>> SearchAvailableTrucksAsync(
            decimal originLatitude, 
            decimal originLongitude, 
            decimal destinationLatitude, 
            decimal destinationLongitude, 
            decimal weight, 
            decimal? height, 
            decimal? width, 
            decimal? length, 
            DateTime pickupDate, 
            DateTime deliveryDate);
        
        // Booking management
        Task<Booking> CreateBookingRequestAsync(long loadId, long truckId, string shipperId);
        Task<IEnumerable<Booking>> GetShipperBookingsAsync(string shipperId);
        Task<Booking?> GetShipperBookingByIdAsync(string shipperId, long bookingId);
        Task<bool> CancelBookingAsync(long bookingId, string shipperId);
        
        // Tracking
        Task<TruckLocation?> GetCurrentLoadLocationAsync(long loadId, string shipperId);
        Task<IEnumerable<object>> GetLoadLocationHistoryAsync(long loadId, string shipperId);
    }
} 