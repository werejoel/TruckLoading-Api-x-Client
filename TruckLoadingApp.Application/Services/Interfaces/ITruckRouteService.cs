using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckRouteService
    {
        Task<IEnumerable<TruckRoute>> GetAllRoutesAsync();
        Task<TruckRoute?> GetRouteByIdAsync(long id);
        Task<IEnumerable<TruckRoute>> GetRoutesByTruckIdAsync(long truckId);
        Task<TruckRoute> CreateRouteAsync(TruckRoute route);
        Task<bool> UpdateRouteAsync(TruckRoute route);
        Task<bool> DeleteRouteAsync(long id);
        Task<bool> ActivateRouteAsync(long id);
        Task<bool> DeactivateRouteAsync(long id);
        Task<TruckRouteWaypoint> AddWaypointAsync(TruckRouteWaypoint waypoint);
        Task<bool> UpdateWaypointAsync(TruckRouteWaypoint waypoint);
        Task<bool> DeleteWaypointAsync(long waypointId);
        Task<IEnumerable<TruckRouteWaypoint>> GetWaypointsByRouteIdAsync(long routeId);
        
        // New method to find trucks that can service a load based on their routes
        Task<IEnumerable<Truck>> FindTrucksForLoadAsync(long loadId, double maxDistanceKm = 50);
    }
} 