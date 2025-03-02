using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverRoutePreferenceService
    {
        Task<DriverRoutePreference> SetRoutePreferencesAsync(DriverRoutePreference preferences);
        Task<DriverRoutePreference?> GetRoutePreferencesAsync(long driverId);
        Task<bool> UpdateRoutePreferencesAsync(DriverRoutePreference preferences);
        Task<IEnumerable<Driver>> FindDriversByRoutePreferencesAsync(RouteRequirements requirements);
    }
}
