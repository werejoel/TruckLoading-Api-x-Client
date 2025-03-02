using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverRoutePreferenceService : IDriverRoutePreferenceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly IDriverScheduleService _driverScheduleService;
        private readonly IDriverComplianceService _driverComplianceService;

        public DriverRoutePreferenceService(
            ApplicationDbContext context,
            IUserActivityService userActivityService,
            IDriverScheduleService driverScheduleService,
            IDriverComplianceService driverComplianceService)
        {
            _context = context;
            _userActivityService = userActivityService;
            _driverScheduleService = driverScheduleService;
            _driverComplianceService = driverComplianceService;
        }

        public async Task<DriverRoutePreference> SetRoutePreferencesAsync(DriverRoutePreference preferences)
        {
            var existingPreferences = await _context.Set<DriverRoutePreference>()
                .FirstOrDefaultAsync(p => p.DriverId == preferences.DriverId);

            if (existingPreferences != null)
            {
                // Update existing preferences
                _context.Entry(existingPreferences).CurrentValues.SetValues(preferences);
                existingPreferences.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                // Create new preferences
                _context.Add(preferences);
            }

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                preferences.Driver.UserId,
                ActivityTypes.UpdateRoutePreferences,
                "Updated route preferences",
                "RoutePreference",
                preferences.Id.ToString());

            return preferences;
        }

        public async Task<DriverRoutePreference?> GetRoutePreferencesAsync(long driverId)
        {
            return await _context.Set<DriverRoutePreference>()
                .Include(p => p.Driver)
                .FirstOrDefaultAsync(p => p.DriverId == driverId);
        }

        public async Task<bool> UpdateRoutePreferencesAsync(DriverRoutePreference preferences)
        {
            var existing = await _context.Set<DriverRoutePreference>()
                .FindAsync(preferences.Id);

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(preferences);
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                existing.Driver.UserId,
                ActivityTypes.UpdateRoutePreferences,
                "RoutePreference",
                preferences.Id.ToString());

            return true;
        }

        public async Task<IEnumerable<Driver>> FindDriversByRoutePreferencesAsync(RouteRequirements requirements)
        {
            var query = _context.Set<Driver>()
                .Include(d => d.User)
                .Where(d => d.IsAvailable);

            if (!string.IsNullOrEmpty(requirements.Region))
            {
                query = query.Where(d => d.RoutePreferences.PreferredRegions != null &&
                                       d.RoutePreferences.PreferredRegions.Contains(requirements.Region));
            }

            if (requirements.StartTime.HasValue && requirements.EndTime.HasValue)
            {
                var availableDrivers = await _driverComplianceService.GetAvailableDriversForTimeSlotAsync(requirements.StartTime.Value, requirements.EndTime.Value);

                query = query.Where(d => availableDrivers.Any(ad => ad.DriverId == d.Id));
            }

            if (requirements.LoadWeight.HasValue)
            {
                query = query.Where(d => d.RoutePreferences.MaxPreferredWeight == null ||
                                       d.RoutePreferences.MaxPreferredWeight >= requirements.LoadWeight);
            }

            if (!string.IsNullOrEmpty(requirements.LoadType))
            {
                query = query.Where(d => d.RoutePreferences.PreferredLoadTypes != null &&
                                       d.RoutePreferences.PreferredLoadTypes.Contains(requirements.LoadType));
            }

            if (requirements.RequireNightDriving)
            {
                query = query.Where(d => !d.RoutePreferences.AvoidNightDriving);
            }

            if (requirements.HasSevereWeather)
            {
                query = query.Where(d => !d.RoutePreferences.AvoidSevereWeather);
            }

            return await query.ToListAsync();
        }
    }

}
