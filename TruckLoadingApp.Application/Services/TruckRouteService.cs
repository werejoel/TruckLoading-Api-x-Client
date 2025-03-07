using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckLoadingApp.Application.Services
{
    public class TruckRouteService : ITruckRouteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckRouteService> _logger;

        // Constants for time window constraints
        private const double DefaultAverageSpeedKmh = 60.0;
        private const int MinimumRequiredBufferMinutes = 30; // 30 minutes buffer time between stops

        public TruckRouteService(ApplicationDbContext context, ILogger<TruckRouteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TruckRoute>> GetAllRoutesAsync()
        {
            return await _context.TruckRoutes
                .Include(r => r.Truck)
                .ToListAsync();
        }

        public async Task<TruckRoute?> GetRouteByIdAsync(long id)
        {
            return await _context.TruckRoutes
                .Include(r => r.Truck)
                .Include(r => r.Waypoints.OrderBy(w => w.SequenceNumber))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TruckRoute>> GetRoutesByTruckIdAsync(long truckId)
        {
            return await _context.TruckRoutes
                .Include(r => r.Truck)
                .Include(r => r.Waypoints.OrderBy(w => w.SequenceNumber))
                .Where(r => r.TruckId == truckId)
                .ToListAsync();
        }

        public async Task<TruckRoute> CreateRouteAsync(TruckRoute route)
        {
            _logger.LogInformation($"Creating a new route for Truck ID: {route.TruckId}");

            route.CreatedDate = DateTime.UtcNow;
            _context.TruckRoutes.Add(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Route created successfully with ID: {route.Id}");

            return route;
        }

        public async Task<bool> UpdateRouteAsync(TruckRoute route)
        {
            var existingRoute = await _context.TruckRoutes.FindAsync(route.Id);
            if (existingRoute == null)
                return false;

            _logger.LogInformation($"Updating route ID: {route.Id}");

            // Update properties
            _context.Entry(existingRoute).CurrentValues.SetValues(route);
            existingRoute.UpdatedDate = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Route updated successfully");
            else
                _logger.LogWarning("Failed to update route");

            return result;
        }

        public async Task<bool> DeleteRouteAsync(long id)
        {
            var route = await _context.TruckRoutes.FindAsync(id);
            if (route == null)
                return false;

            _logger.LogInformation($"Deleting route ID: {id}");

            _context.TruckRoutes.Remove(route);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Route deleted successfully");
            else
                _logger.LogWarning("Failed to delete route");

            return result;
        }

        public async Task<bool> ActivateRouteAsync(long id)
        {
            var route = await _context.TruckRoutes.FindAsync(id);
            if (route == null)
                return false;

            _logger.LogInformation($"Activating route ID: {id}");

            route.IsActive = true;
            route.UpdatedDate = DateTime.UtcNow;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Route activated successfully");
            else
                _logger.LogWarning("Failed to activate route");

            return result;
        }

        public async Task<bool> DeactivateRouteAsync(long id)
        {
            var route = await _context.TruckRoutes.FindAsync(id);
            if (route == null)
                return false;

            _logger.LogInformation($"Deactivating route ID: {id}");

            route.IsActive = false;
            route.UpdatedDate = DateTime.UtcNow;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Route deactivated successfully");
            else
                _logger.LogWarning("Failed to deactivate route");

            return result;
        }

        public async Task<TruckRouteWaypoint> AddWaypointAsync(TruckRouteWaypoint waypoint)
        {
            _logger.LogInformation($"Adding waypoint to route ID: {waypoint.TruckRouteId}");

            waypoint.CreatedDate = DateTime.UtcNow;
            _context.TruckRouteWaypoints.Add(waypoint);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Waypoint added successfully with ID: {waypoint.Id}");

            return waypoint;
        }

        public async Task<bool> UpdateWaypointAsync(TruckRouteWaypoint waypoint)
        {
            var existingWaypoint = await _context.TruckRouteWaypoints.FindAsync(waypoint.Id);
            if (existingWaypoint == null)
                return false;

            _logger.LogInformation($"Updating waypoint ID: {waypoint.Id}");

            // Update properties
            _context.Entry(existingWaypoint).CurrentValues.SetValues(waypoint);
            existingWaypoint.UpdatedDate = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Waypoint updated successfully");
            else
                _logger.LogWarning("Failed to update waypoint");

            return result;
        }

        public async Task<bool> DeleteWaypointAsync(long waypointId)
        {
            var waypoint = await _context.TruckRouteWaypoints.FindAsync(waypointId);
            if (waypoint == null)
                return false;

            _logger.LogInformation($"Deleting waypoint ID: {waypointId}");

            _context.TruckRouteWaypoints.Remove(waypoint);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Waypoint deleted successfully");
            else
                _logger.LogWarning("Failed to delete waypoint");

            return result;
        }

        public async Task<IEnumerable<TruckRouteWaypoint>> GetWaypointsByRouteIdAsync(long routeId)
        {
            return await _context.TruckRouteWaypoints
                .Where(w => w.TruckRouteId == routeId)
                .OrderBy(w => w.SequenceNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Truck>> FindTrucksForLoadAsync(long loadId, double maxDistanceKm = 50)
        {
            _logger.LogInformation($"Finding trucks that can service load ID: {loadId}");

            // Get the load details
            var load = await _context.Loads
                .Include(l => l.RequiredTruckType)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null)
            {
                _logger.LogWarning($"Load ID: {loadId} not found");
                return new List<Truck>();
            }

            // Get all active truck routes
            var activeRoutes = await _context.TruckRoutes
                .Include(r => r.Truck)
                    .ThenInclude(t => t.TruckType)
                .Include(r => r.Truck)
                    .ThenInclude(t => t.AssignedDriver)
                .Include(r => r.Waypoints.OrderBy(w => w.SequenceNumber))
                .Where(r => r.IsActive && 
                       r.StartDate <= load.PickupDate && 
                       (r.EndDate == null || r.EndDate >= load.DeliveryDate))
                .ToListAsync();

            var matchingTrucks = new List<Truck>();
            var processedTruckIds = new HashSet<int>();

            foreach (var route in activeRoutes)
            {
                var truck = route.Truck;

                // Skip if we've already processed this truck or if it's not approved or not active
                if (processedTruckIds.Contains(truck.Id) || 
                    !truck.IsApproved || 
                    truck.OperationalStatus != TruckOperationalStatusEnum.Active)
                {
                    continue;
                }

                // Check if truck meets basic requirements
                if (truck.AvailableCapacityWeight < load.Weight)
                {
                    continue;
                }

                // Check if truck type matches required truck type (if specified)
                if (load.RequiredTruckTypeId.HasValue && 
                    truck.TruckTypeId != load.RequiredTruckTypeId.Value)
                {
                    continue;
                }

                // Check if truck has required dimensions (if specified)
                if ((load.Height.HasValue && truck.Height.HasValue && truck.Height < load.Height) ||
                    (load.Width.HasValue && truck.Width.HasValue && truck.Width < load.Width) ||
                    (load.Length.HasValue && truck.Length.HasValue && truck.Length < load.Length))
                {
                    continue;
                }

                // Check if truck has required features
                if ((load.RequiresTemperatureControl && !truck.HasRefrigeration) ||
                    (load.HazardousMaterialClass != HazardousMaterialClass.None && !truck.CanTransportHazardousMaterials))
                {
                    continue;
                }

                // Check if the truck's route matches the load spatially and temporally
                var (isRouteMatching, timeCompatibilityScore) = IsRouteMatchingLoad(route, load, maxDistanceKm);
                
                if (isRouteMatching)
                {
                    // Calculate distance to pickup for sorting purposes
                    truck.DistanceToPickup = CalculateMinimumDistanceToWaypoint(
                        route.Waypoints, 
                        load.PickupLatitude ?? 0, 
                        load.PickupLongitude ?? 0);
                    
                    // Store the time compatibility score as a property on the truck for sorting
                    truck.RouteDistance = timeCompatibilityScore;
                    
                    matchingTrucks.Add(truck);
                    processedTruckIds.Add(truck.Id);
                }
            }

            // Sort by time compatibility score first, then by distance to pickup
            return matchingTrucks
                .OrderByDescending(t => t.RouteDistance)  // Higher score is better
                .ThenBy(t => t.DistanceToPickup);         // Lower distance is better
        }

        private (bool isMatching, decimal timeCompatibilityScore) IsRouteMatchingLoad(TruckRoute route, Load load, double maxDistanceKm)
        {
            if (route.Waypoints == null || !route.Waypoints.Any() || 
                !load.PickupLatitude.HasValue || !load.PickupLongitude.HasValue ||
                !load.DeliveryLatitude.HasValue || !load.DeliveryLongitude.HasValue)
            {
                return (false, 0);
            }

            // Order waypoints by sequence number to ensure proper time window analysis
            var orderedWaypoints = route.Waypoints.OrderBy(w => w.SequenceNumber).ToList();

            // Find closest waypoint to pickup location
            var closestToPickup = FindClosestWaypoint(
                orderedWaypoints, 
                (double)load.PickupLatitude.Value, 
                (double)load.PickupLongitude.Value,
                maxDistanceKm);

            // Find closest waypoint to delivery location
            var closestToDelivery = FindClosestWaypoint(
                orderedWaypoints, 
                (double)load.DeliveryLatitude.Value, 
                (double)load.DeliveryLongitude.Value,
                maxDistanceKm);

            if (closestToPickup == null || closestToDelivery == null)
            {
                return (false, 0);
            }

            // Check if the closest waypoint to pickup is before the closest waypoint to delivery in the route sequence
            if (closestToPickup.SequenceNumber > closestToDelivery.SequenceNumber)
            {
                _logger.LogInformation($"Route {route.Id} doesn't match load {load.Id} due to waypoint sequence mismatch");
                return (false, 0);
            }

            // Check time windows compatibility
            var timeCompatibility = CheckTimeWindowCompatibility(
                orderedWaypoints,
                closestToPickup,
                closestToDelivery,
                load.PickupDate,
                load.DeliveryDate,
                (double)load.PickupLatitude.Value,
                (double)load.PickupLongitude.Value,
                (double)load.DeliveryLatitude.Value,
                (double)load.DeliveryLongitude.Value);

            return timeCompatibility;
        }

        private TruckRouteWaypoint? FindClosestWaypoint(
            IEnumerable<TruckRouteWaypoint> waypoints, 
            double latitude, 
            double longitude,
            double maxDistance)
        {
            TruckRouteWaypoint? closestWaypoint = null;
            double minDistance = maxDistance;

            foreach (var waypoint in waypoints)
            {
                double distance = CalculateDistance(
                    (double)waypoint.Latitude, 
                    (double)waypoint.Longitude, 
                    latitude, 
                    longitude);

                if (distance <= minDistance)
                {
                    minDistance = distance;
                    closestWaypoint = waypoint;
                }
            }

            return closestWaypoint;
        }

        private (bool isCompatible, decimal compatibilityScore) CheckTimeWindowCompatibility(
            List<TruckRouteWaypoint> orderedWaypoints,
            TruckRouteWaypoint pickupWaypoint,
            TruckRouteWaypoint deliveryWaypoint,
            DateTime requiredPickupTime,
            DateTime requiredDeliveryTime,
            double pickupLat,
            double pickupLon,
            double deliveryLat,
            double deliveryLon)
        {
            // If waypoints don't have estimated times, they're not suitable for time window matching
            if (!pickupWaypoint.EstimatedArrivalTime.HasValue || !deliveryWaypoint.EstimatedArrivalTime.HasValue)
            {
                _logger.LogInformation("Waypoints don't have estimated arrival times for proper time window matching");
                return (false, 0);
            }

            // Calculate estimated time needed for pickup detour
            int pickupIndex = orderedWaypoints.FindIndex(w => w.Id == pickupWaypoint.Id);
            if (pickupIndex < 0 || pickupIndex >= orderedWaypoints.Count - 1)
            {
                return (false, 0);
            }

            var nextWaypoint = orderedWaypoints[pickupIndex + 1];
            
            // Calculate detour time to include pickup location
            var originalTime = EstimateTimeToTravel(
                (double)pickupWaypoint.Latitude, 
                (double)pickupWaypoint.Longitude,
                (double)nextWaypoint.Latitude, 
                (double)nextWaypoint.Longitude);
            
            var detourTime = EstimateTimeToTravel(
                (double)pickupWaypoint.Latitude, 
                (double)pickupWaypoint.Longitude,
                pickupLat, 
                pickupLon) +
                EstimateTimeToTravel(
                    pickupLat, 
                    pickupLon, 
                    (double)nextWaypoint.Latitude, 
                    (double)nextWaypoint.Longitude);

            // Additional time needed for the detour (minutes)
            var additionalPickupTime = (detourTime - originalTime).TotalMinutes;
            
            // Add loading/unloading time (assume 30 minutes)
            additionalPickupTime += 30;
            
            // Calculate the same for delivery
            int deliveryIndex = orderedWaypoints.FindIndex(w => w.Id == deliveryWaypoint.Id);
            if (deliveryIndex <= pickupIndex || deliveryIndex >= orderedWaypoints.Count - 1)
            {
                return (false, 0);
            }
            
            var nextDeliveryWaypoint = orderedWaypoints[deliveryIndex + 1];
            
            var originalDeliveryTime = EstimateTimeToTravel(
                (double)deliveryWaypoint.Latitude, 
                (double)deliveryWaypoint.Longitude,
                (double)nextDeliveryWaypoint.Latitude, 
                (double)nextDeliveryWaypoint.Longitude);
            
            var deliveryDetourTime = EstimateTimeToTravel(
                (double)deliveryWaypoint.Latitude, 
                (double)deliveryWaypoint.Longitude,
                deliveryLat, 
                deliveryLon) +
                EstimateTimeToTravel(
                    deliveryLat, 
                    deliveryLon, 
                    (double)nextDeliveryWaypoint.Latitude, 
                    (double)nextDeliveryWaypoint.Longitude);

            // Additional time needed for the detour (minutes)
            var additionalDeliveryTime = (deliveryDetourTime - originalDeliveryTime).TotalMinutes + 30; // 30 mins for unloading
            
            // Check if there's enough buffer time in the truck's schedule
            DateTime estimatedPickupTime = pickupWaypoint.EstimatedArrivalTime.Value;
            DateTime estimatedDeliveryTime = deliveryWaypoint.EstimatedArrivalTime.Value;
            
            // Next waypoint's estimated time
            DateTime nextWaypointTime = nextWaypoint.EstimatedArrivalTime.GetValueOrDefault(
                estimatedPickupTime.AddMinutes(
                    EstimateTimeToTravel(
                        (double)pickupWaypoint.Latitude, 
                        (double)pickupWaypoint.Longitude,
                        (double)nextWaypoint.Latitude, 
                        (double)nextWaypoint.Longitude).TotalMinutes));
            
            // Calculate available buffer for pickup
            double pickupBufferMinutes = (nextWaypointTime - estimatedPickupTime).TotalMinutes - 
                (pickupWaypoint.StopDurationMinutes ?? 0);
            
            // Check if there's enough time for the pickup detour
            if (pickupBufferMinutes < additionalPickupTime + MinimumRequiredBufferMinutes)
            {
                _logger.LogInformation($"Not enough buffer time for pickup: available {pickupBufferMinutes}min, needed {additionalPickupTime + MinimumRequiredBufferMinutes}min");
                return (false, 0);
            }
            
            // Next delivery waypoint's estimated time
            DateTime nextDeliveryWaypointTime = nextDeliveryWaypoint.EstimatedArrivalTime.GetValueOrDefault(
                estimatedDeliveryTime.AddMinutes(
                    EstimateTimeToTravel(
                        (double)deliveryWaypoint.Latitude, 
                        (double)deliveryWaypoint.Longitude,
                        (double)nextDeliveryWaypoint.Latitude, 
                        (double)nextDeliveryWaypoint.Longitude).TotalMinutes));
            
            // Calculate available buffer for delivery
            double deliveryBufferMinutes = (nextDeliveryWaypointTime - estimatedDeliveryTime).TotalMinutes -
                (deliveryWaypoint.StopDurationMinutes ?? 0);
            
            // Check if there's enough time for the delivery detour
            if (deliveryBufferMinutes < additionalDeliveryTime + MinimumRequiredBufferMinutes)
            {
                _logger.LogInformation($"Not enough buffer time for delivery: available {deliveryBufferMinutes}min, needed {additionalDeliveryTime + MinimumRequiredBufferMinutes}min");
                return (false, 0);
            }
            
            // Check if the required pickup and delivery times fit within the truck's schedule
            if (requiredPickupTime < estimatedPickupTime || requiredDeliveryTime > estimatedDeliveryTime)
            {
                _logger.LogInformation($"Required pickup/delivery times don't fit within truck's schedule");
                return (false, 0);
            }
            
            // Calculate a compatibility score (0-100) based on how well the times match and buffer availability
            decimal pickupScore = (decimal)Math.Min(100, (pickupBufferMinutes / (additionalPickupTime + MinimumRequiredBufferMinutes)) * 100);
            decimal deliveryScore = (decimal)Math.Min(100, (deliveryBufferMinutes / (additionalDeliveryTime + MinimumRequiredBufferMinutes)) * 100);
            
            // Combine both scores with weights (pickup being slightly more important)
            decimal combinedScore = (pickupScore * 0.6M) + (deliveryScore * 0.4M);
            
            _logger.LogInformation($"Route is compatible with time windows. Score: {combinedScore}");
            return (true, combinedScore);
        }

        private TimeSpan EstimateTimeToTravel(double lat1, double lon1, double lat2, double lon2)
        {
            // Calculate distance between points
            double distanceKm = CalculateDistance(lat1, lon1, lat2, lon2);
            
            // Calculate time based on average speed
            double hours = distanceKm / DefaultAverageSpeedKmh;
            
            return TimeSpan.FromHours(hours);
        }

        private decimal CalculateMinimumDistanceToWaypoint(
            ICollection<TruckRouteWaypoint> waypoints, 
            decimal loadLatitude, 
            decimal loadLongitude)
        {
            if (waypoints == null || !waypoints.Any())
            {
                return decimal.MaxValue;
            }

            decimal minDistance = decimal.MaxValue;

            foreach (var waypoint in waypoints)
            {
                decimal distance = (decimal)CalculateDistance(
                    (double)waypoint.Latitude, 
                    (double)waypoint.Longitude, 
                    (double)loadLatitude, 
                    (double)loadLongitude);

                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}