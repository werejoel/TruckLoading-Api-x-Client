using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.API.Services;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class ShipperService : IShipperService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShipperService> _logger;
        private readonly IBookingService _bookingService;
        private readonly API.Services.ITruckLocationService _truckLocationService;

        public ShipperService(
            ApplicationDbContext context,
            ILogger<ShipperService> logger,
            IBookingService bookingService,
            ITruckLocationService truckLocationService)
        {
            _context = context;
            _logger = logger;
            _bookingService = bookingService;
            _truckLocationService = truckLocationService;
        }

        // Load management
        public async Task<Load> CreateLoadAsync(Load load, string shipperId)
        {
            _logger.LogInformation($"Creating a new load for Shipper ID: {shipperId}");

            // Ensure the load is associated with the correct shipper
            load.ShipperId = shipperId;
            load.Status = LoadStatusEnum.Available;
            load.CreatedDate = DateTime.UtcNow;

            _context.Loads.Add(load);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Load created successfully with ID: {load.Id}");

            return load;
        }

        public async Task<IEnumerable<Load>> GetShipperLoadsAsync(string shipperId)
        {
            _logger.LogInformation($"Retrieving loads for Shipper ID: {shipperId}");

            return await _context.Loads
                .Where(l => l.ShipperId == shipperId)
                .Include(l => l.LoadType)
                .Include(l => l.RequiredTruckType)
                .ToListAsync();
        }

        public async Task<Load?> GetShipperLoadByIdAsync(string shipperId, long loadId)
        {
            _logger.LogInformation($"Retrieving load ID: {loadId} for Shipper ID: {shipperId}");

            return await _context.Loads
                .Include(l => l.LoadType)
                .Include(l => l.RequiredTruckType)
                .Include(l => l.LoadDimensions)
                .Include(l => l.TemperatureRequirement)
                .Include(l => l.LoadTags)
                    .ThenInclude(lt => lt.LoadTag)
                .FirstOrDefaultAsync(l => l.Id == loadId && l.ShipperId == shipperId);
        }

        public async Task<bool> UpdateLoadAsync(Load load, string shipperId)
        {
            _logger.LogInformation($"Updating load ID: {load.Id} for Shipper ID: {shipperId}");

            var existingLoad = await _context.Loads
                .FirstOrDefaultAsync(l => l.Id == load.Id && l.ShipperId == shipperId);

            if (existingLoad == null)
            {
                _logger.LogWarning($"Load ID: {load.Id} not found for Shipper ID: {shipperId}");
                return false;
            }

            // Don't allow updating if the load is already booked or in transit
            if (existingLoad.Status != LoadStatusEnum.Available && 
                existingLoad.Status != LoadStatusEnum.Draft)
            {
                _logger.LogWarning($"Cannot update load ID: {load.Id} with status: {existingLoad.Status}");
                return false;
            }

            // Update properties
            existingLoad.Weight = load.Weight;
            existingLoad.Height = load.Height;
            existingLoad.Width = load.Width;
            existingLoad.Length = load.Length;
            existingLoad.Description = load.Description;
            existingLoad.PickupDate = load.PickupDate;
            existingLoad.DeliveryDate = load.DeliveryDate;
            existingLoad.SpecialRequirements = load.SpecialRequirements;
            existingLoad.GoodsType = load.GoodsType;
            existingLoad.LoadTypeId = load.LoadTypeId;
            existingLoad.Price = load.Price;
            existingLoad.Currency = load.Currency;
            existingLoad.Region = load.Region;
            existingLoad.RequiredTruckTypeId = load.RequiredTruckTypeId;
            existingLoad.IsStackable = load.IsStackable;
            existingLoad.RequiresTemperatureControl = load.RequiresTemperatureControl;
            existingLoad.HazardousMaterialClass = load.HazardousMaterialClass;
            existingLoad.HandlingInstructions = load.HandlingInstructions;
            existingLoad.IsFragile = load.IsFragile;
            existingLoad.RequiresStackingControl = load.RequiresStackingControl;
            existingLoad.StackingInstructions = load.StackingInstructions;
            existingLoad.UNNumber = load.UNNumber;
            existingLoad.RequiresCustomsDeclaration = load.RequiresCustomsDeclaration;
            existingLoad.CustomsDeclarationNumber = load.CustomsDeclarationNumber;
            existingLoad.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Load ID: {load.Id} updated successfully");

            return true;
        }

        public async Task<bool> DeleteLoadAsync(long loadId, string shipperId)
        {
            _logger.LogInformation($"Deleting load ID: {loadId} for Shipper ID: {shipperId}");

            var load = await _context.Loads
                .FirstOrDefaultAsync(l => l.Id == loadId && l.ShipperId == shipperId);

            if (load == null)
            {
                _logger.LogWarning($"Load ID: {loadId} not found for Shipper ID: {shipperId}");
                return false;
            }

            // Don't allow deleting if the load is already booked or in transit
            if (load.Status != LoadStatusEnum.Available && 
                load.Status != LoadStatusEnum.Draft)
            {
                _logger.LogWarning($"Cannot delete load ID: {loadId} with status: {load.Status}");
                return false;
            }

            _context.Loads.Remove(load);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Load ID: {loadId} deleted successfully");

            return true;
        }

        // Truck search
        public async Task<IEnumerable<Truck>> SearchAvailableTrucksAsync(
            decimal originLatitude, 
            decimal originLongitude, 
            decimal destinationLatitude, 
            decimal destinationLongitude, 
            decimal weight, 
            decimal? height, 
            decimal? width, 
            decimal? length, 
            DateTime pickupDate, 
            DateTime deliveryDate)
        {
            _logger.LogInformation("Searching for available trucks");

            // Get all available trucks
            var availableTrucks = await _context.Trucks
                .Where(t => t.IsApproved && 
                       t.OperationalStatus == TruckOperationalStatusEnum.Active &&
                       t.AvailabilityStartDate <= pickupDate &&
                       t.AvailabilityEndDate >= deliveryDate &&
                       t.AvailableCapacityWeight >= weight)
                .Include(t => t.TruckType)
                .Include(t => t.AssignedDriver)
                .ToListAsync();

            // Filter by dimensions if provided
            if (height.HasValue)
            {
                availableTrucks = availableTrucks
                    .Where(t => t.Height >= height.Value)
                    .ToList();
            }

            if (width.HasValue)
            {
                availableTrucks = availableTrucks
                    .Where(t => t.Width >= width.Value)
                    .ToList();
            }

            if (length.HasValue)
            {
                availableTrucks = availableTrucks
                    .Where(t => t.Length >= length.Value)
                    .ToList();
            }

            // Implement basic route matching algorithm
            var enhancedTrucks = new List<Truck>();
            
            foreach (var truck in availableTrucks)
            {
                // Get the truck's current location
                var truckLocation = await _truckLocationService.GetCurrentTruckLocationAsync(truck.Id);
                
                if (truckLocation != null)
                {
                    // Calculate distance from truck's current location to pickup point
                    double distanceToPickup = CalculateDistance(
                        (double)truckLocation.CurrentLatitude, 
                        (double)truckLocation.CurrentLongitude, 
                        (double)originLatitude, 
                        (double)originLongitude);
                    
                    // Calculate distance from pickup to delivery
                    double routeDistance = CalculateDistance(
                        (double)originLatitude, 
                        (double)originLongitude, 
                        (double)destinationLatitude, 
                        (double)destinationLongitude);
                    
                    // If the truck is within a reasonable distance (e.g., 100km) of the pickup point
                    // or if the truck's route is similar to the requested route, add it to the results
                    if (distanceToPickup <= 100 || IsRouteMatching(truck, originLatitude, originLongitude, destinationLatitude, destinationLongitude))
                    {
                        // Store the calculated distances as properties on the truck for later use
                        truck.DistanceToPickup = (decimal)distanceToPickup;
                        truck.RouteDistance = (decimal)routeDistance;
                        
                        enhancedTrucks.Add(truck);
                    }
                }
                else
                {
                    // If we don't have location data for the truck, still include it in results
                    // but mark it as having unknown location
                    truck.DistanceToPickup = -1; // -1 indicates unknown distance
                    enhancedTrucks.Add(truck);
                }
            }
            
            // Sort trucks by distance to pickup (closest first)
            var sortedTrucks = enhancedTrucks
                .OrderBy(t => t.DistanceToPickup == -1 ? decimal.MaxValue : t.DistanceToPickup)
                .ToList();

            _logger.LogInformation($"Found {sortedTrucks.Count} available trucks");
            return sortedTrucks;
        }

        // Helper method to calculate distance between two points using Haversine formula
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;
            
            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // Helper method to determine if a truck's route matches the requested route
        private bool IsRouteMatching(Truck truck, decimal originLat, decimal originLon, decimal destLat, decimal destLon)
        {
            // This is a simplified implementation
            // In a real-world scenario, you would check the truck's planned route
            // and see if it passes near the pickup and delivery points
            
            // For now, we'll just check if the truck has any active bookings
            // with routes that are similar to the requested route
            var activeBookings = _context.Bookings
                .Where(b => b.TruckId == truck.Id && 
                           (b.Status == BookingStatusEnum.Confirmed || 
                            b.Status == BookingStatusEnum.InProgress))
                .Include(b => b.Load)
                .ToList();
            
            foreach (var booking in activeBookings)
            {
                if (booking.Load != null)
                {
                    // Get the load's pickup and delivery locations
                    // This assumes you have latitude and longitude stored for loads
                    // If not, you would need to modify this logic
                    
                    // For now, we'll just return true if there are any active bookings
                    // In a real implementation, you would compare routes
                    return true;
                }
            }
            
            return false;
        }

        // Booking management
        public async Task<Booking> CreateBookingRequestAsync(long loadId, long truckId, string shipperId)
        {
            _logger.LogInformation($"Creating booking request for Load ID: {loadId}, Truck ID: {truckId}, Shipper ID: {shipperId}");

            // Verify the load belongs to the shipper
            var load = await _context.Loads
                .FirstOrDefaultAsync(l => l.Id == loadId && l.ShipperId == shipperId);

            if (load == null)
            {
                _logger.LogWarning($"Load ID: {loadId} not found for Shipper ID: {shipperId}");
                throw new ArgumentException($"Load ID: {loadId} not found for Shipper ID: {shipperId}");
            }

            // Verify the truck is available
            var truck = await _context.Trucks
                .FirstOrDefaultAsync(t => t.Id == truckId && 
                                    t.IsApproved && 
                                    t.OperationalStatus == TruckOperationalStatusEnum.Active);

            if (truck == null)
            {
                _logger.LogWarning($"Truck ID: {truckId} not found or not available");
                throw new ArgumentException($"Truck ID: {truckId} not found or not available");
            }

            // Calculate price (simplified for now)
            decimal agreedPrice = load.Price ?? 0;
            
            // Create the booking
            var booking = await _bookingService.CreateBooking(
                load, 
                truck, 
                agreedPrice, 
                "StandardRate", 
                load.Currency ?? "USD");

            // Update load status
            load.Status = LoadStatusEnum.PendingBooking;
            load.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Booking created successfully with ID: {booking.Id}");
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetShipperBookingsAsync(string shipperId)
        {
            _logger.LogInformation($"Retrieving bookings for Shipper ID: {shipperId}");

            return await _context.Bookings
                .Include(b => b.Load)
                .Include(b => b.Truck)
                .Where(b => b.Load != null && b.Load.ShipperId == shipperId)
                .ToListAsync();
        }

        public async Task<Booking?> GetShipperBookingByIdAsync(string shipperId, long bookingId)
        {
            _logger.LogInformation($"Retrieving booking ID: {bookingId} for Shipper ID: {shipperId}");

            return await _context.Bookings
                .Include(b => b.Load)
                .Include(b => b.Truck)
                .FirstOrDefaultAsync(b => b.Id == bookingId && 
                                     b.Load != null && 
                                     b.Load.ShipperId == shipperId);
        }

        public async Task<bool> CancelBookingAsync(long bookingId, string shipperId)
        {
            _logger.LogInformation($"Cancelling booking ID: {bookingId} for Shipper ID: {shipperId}");

            var booking = await _context.Bookings
                .Include(b => b.Load)
                .FirstOrDefaultAsync(b => b.Id == bookingId && 
                                     b.Load != null && 
                                     b.Load.ShipperId == shipperId);

            if (booking == null)
            {
                _logger.LogWarning($"Booking ID: {bookingId} not found for Shipper ID: {shipperId}");
                return false;
            }

            // Don't allow cancelling if the booking is already in progress or completed
            if (booking.Status == BookingStatusEnum.InProgress || 
                booking.Status == BookingStatusEnum.Completed)
            {
                _logger.LogWarning($"Cannot cancel booking ID: {bookingId} with status: {booking.Status}");
                return false;
            }

            // Update booking status
            booking.Status = BookingStatusEnum.Cancelled;
            booking.CancellationReason = "Cancelled by shipper";
            booking.UpdatedDate = DateTime.UtcNow;

            // Update load status
            if (booking.Load != null)
            {
                booking.Load.Status = LoadStatusEnum.Available;
                booking.Load.UpdatedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Booking ID: {bookingId} cancelled successfully");

            return true;
        }

        // Tracking
        public async Task<TruckLocation?> GetCurrentLoadLocationAsync(long loadId, string shipperId)
        {
            _logger.LogInformation($"Retrieving current location for Load ID: {loadId}, Shipper ID: {shipperId}");

            // Verify the load belongs to the shipper and has an active booking
            var booking = await _context.Bookings
                .Include(b => b.Load)
                .Include(b => b.Truck)
                .FirstOrDefaultAsync(b => b.Load != null && 
                                     b.Load.Id == loadId && 
                                     b.Load.ShipperId == shipperId &&
                                     (b.Status == BookingStatusEnum.Confirmed || 
                                      b.Status == BookingStatusEnum.InProgress));

            if (booking == null || booking.Truck == null)
            {
                _logger.LogWarning($"No active booking found for Load ID: {loadId}, Shipper ID: {shipperId}");
                return null;
            }

            // Get the truck's current location
            return await _truckLocationService.GetCurrentTruckLocationAsync(booking.Truck.Id);
        }

        public async Task<IEnumerable<object>> GetLoadLocationHistoryAsync(long loadId, string shipperId)
        {
            _logger.LogInformation($"Retrieving location history for Load ID: {loadId}, Shipper ID: {shipperId}");

            // Verify the load belongs to the shipper and has an active booking
            var booking = await _context.Bookings
                .Include(b => b.Load)
                .Include(b => b.Truck)
                .FirstOrDefaultAsync(b => b.Load != null && 
                                     b.Load.Id == loadId && 
                                     b.Load.ShipperId == shipperId);

            if (booking == null || booking.Truck == null)
            {
                _logger.LogWarning($"No booking found for Load ID: {loadId}, Shipper ID: {shipperId}");
                return new List<object>();
            }

            // Get the truck's location history
            var history = await _truckLocationService.GetTruckHistoryAsync((int)booking.Truck.Id);
            return history ?? new List<object>();
        }
    }
} 