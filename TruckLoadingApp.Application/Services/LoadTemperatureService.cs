using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class LoadTemperatureService : ILoadTemperatureService
    {
        private readonly ApplicationDbContext _context;

        public LoadTemperatureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoadTemperatureRequirement> CreateTemperatureRequirementAsync(
            long loadId,
            decimal minTemperature,
            decimal maxTemperature,
            string temperatureUnit = "°C",
            bool requiresContinuousMonitoring = false,
            int? monitoringIntervalMinutes = null)
        {
            var load = await _context.Loads.FindAsync(loadId);
            if (load == null)
            {
                throw new KeyNotFoundException($"Load with ID {loadId} not found.");
            }

            if (minTemperature >= maxTemperature)
            {
                throw new ArgumentException("Minimum temperature must be less than maximum temperature.");
            }

            var requirement = new LoadTemperatureRequirement
            {
                LoadId = loadId,
                MinTemperature = minTemperature,
                MaxTemperature = maxTemperature,
                TemperatureUnit = temperatureUnit,
                RequiresContinuousMonitoring = requiresContinuousMonitoring,
                MonitoringIntervalMinutes = monitoringIntervalMinutes
            };

            _context.LoadTemperatureRequirements.Add(requirement);
            await _context.SaveChangesAsync();

            return requirement;
        }

        public async Task<LoadTemperatureRequirement?> GetTemperatureRequirementAsync(long loadId)
        {
            return await _context.LoadTemperatureRequirements
                .FirstOrDefaultAsync(tr => tr.LoadId == loadId);
        }

        public async Task<LoadTemperatureRequirement> UpdateTemperatureRequirementAsync(
            long loadId,
            decimal minTemperature,
            decimal maxTemperature,
            string temperatureUnit = "°C",
            bool requiresContinuousMonitoring = false,
            int? monitoringIntervalMinutes = null)
        {
            var requirement = await _context.LoadTemperatureRequirements
                .FirstOrDefaultAsync(tr => tr.LoadId == loadId);

            if (requirement == null)
            {
                throw new KeyNotFoundException($"Temperature requirement for load ID {loadId} not found.");
            }

            if (minTemperature >= maxTemperature)
            {
                throw new ArgumentException("Minimum temperature must be less than maximum temperature.");
            }

            requirement.MinTemperature = minTemperature;
            requirement.MaxTemperature = maxTemperature;
            requirement.TemperatureUnit = temperatureUnit;
            requirement.RequiresContinuousMonitoring = requiresContinuousMonitoring;
            requirement.MonitoringIntervalMinutes = monitoringIntervalMinutes;

            await _context.SaveChangesAsync();
            return requirement;
        }

        public async Task<bool> DeleteTemperatureRequirementAsync(long loadId)
        {
            var requirement = await _context.LoadTemperatureRequirements
                .FirstOrDefaultAsync(tr => tr.LoadId == loadId);

            if (requirement == null) return false;

            _context.LoadTemperatureRequirements.Remove(requirement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTemperatureInRangeAsync(long loadId, decimal currentTemperature)
        {
            var requirement = await _context.LoadTemperatureRequirements
                .FirstOrDefaultAsync(tr => tr.LoadId == loadId);

            if (requirement == null) return true; // No temperature requirements

            return currentTemperature >= requirement.MinTemperature &&
                   currentTemperature <= requirement.MaxTemperature;
        }

        public async Task<IEnumerable<Load>> GetLoadsRequiringTemperatureMonitoringAsync()
        {
            return await _context.Loads
                .Include(l => l.TemperatureRequirement)
                .Where(l => l.RequiresTemperatureControl &&
                           l.TemperatureRequirement != null &&
                           l.TemperatureRequirement.RequiresContinuousMonitoring)
                .ToListAsync();
        }

        public async Task<bool> RecordTemperatureReadingAsync(
            long loadId,
            decimal temperature,
            DateTime timestamp,
            string? deviceId = null)
        {
            var requirement = await _context.LoadTemperatureRequirements
                .FirstOrDefaultAsync(tr => tr.LoadId == loadId);

            if (requirement == null) return false;

            var reading = new TemperatureReading
            {
                LoadId = loadId,
                Temperature = temperature,
                Timestamp = timestamp,
                DeviceId = deviceId,
                IsWithinRange = temperature >= requirement.MinTemperature &&
                               temperature <= requirement.MaxTemperature
            };

            _context.Set<TemperatureReading>().Add(reading);
            await _context.SaveChangesAsync();

            // If temperature is out of range, you might want to trigger notifications here
            if (!reading.IsWithinRange)
            {
                // TODO: Implement notification system for temperature violations
            }

            return true;
        }

        // New methods implementation
        public async Task<IEnumerable<TemperatureReading>> GetTemperatureReadingsAsync(long loadId)
        {
            return await _context.Set<TemperatureReading>()
                .Where(tr => tr.LoadId == loadId)
                .OrderByDescending(tr => tr.Timestamp)
                .ToListAsync();
        }

        public async Task<TemperatureReading?> GetTemperatureReadingByIdAsync(long id)
        {
            return await _context.Set<TemperatureReading>()
                .FindAsync(id);
        }

        public async Task<TemperatureReading?> GetLatestTemperatureReadingAsync(long loadId)
        {
            return await _context.Set<TemperatureReading>()
                .Where(tr => tr.LoadId == loadId)
                .OrderByDescending(tr => tr.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
