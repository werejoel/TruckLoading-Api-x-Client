using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Application.Services
{
    public class TruckRouteService : ITruckRouteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckRouteService> _logger;

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
                .Include(r => r.Waypoints)
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

                // Check if the truck's route passes near the load's pickup and delivery locations
                bool isRouteMatching = IsRouteMatchingLoad(route, load, maxDistanceKm);
                
                if (isRouteMatching)
                {
                    // Calculate distance to pickup for sorting purposes
                    truck.DistanceToPickup = CalculateMinimumDistanceToWaypoint(
                        route.Waypoints, 
                        load.PickupLatitude ?? 0, 
                        load.PickupLongitude ?? 0);
                    
                    matchingTrucks.Add(truck);
                    processedTruckIds.Add(truck.Id);
                }
            }

            // Sort by distance to pickup
            return matchingTrucks.OrderBy(t => t.DistanceToPickup);
        }

        private bool IsRouteMatchingLoad(TruckRoute route, Load load, double maxDistanceKm)
        {
            if (route.Waypoints == null || !route.Waypoints.Any() || 
                !load.PickupLatitude.HasValue || !load.PickupLongitude.HasValue ||
                !load.DeliveryLatitude.HasValue || !load.DeliveryLongitude.HasValue)
            {
                return false;
            }

            // Check if any waypoint is close to the pickup location
            bool isNearPickup = route.Waypoints.Any(w => 
                CalculateDistance(
                    (double)w.Latitude, 
                    (double)w.Longitude, 
                    (double)load.PickupLatitude.Value, 
                    (double)load.PickupLongitude.Value) <= maxDistanceKm);

            // Check if any waypoint is close to the delivery location
            bool isNearDelivery = route.Waypoints.Any(w => 
                CalculateDistance(
                    (double)w.Latitude, 
                    (double)w.Longitude, 
                    (double)load.DeliveryLatitude.Value, 
                    (double)load.DeliveryLongitude.Value) <= maxDistanceKm);

            // Check if the route passes through both pickup and delivery areas
            return isNearPickup && isNearDelivery;
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