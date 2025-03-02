using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ILoadTemperatureService
    {
        Task<LoadTemperatureRequirement> CreateTemperatureRequirementAsync(
            long loadId,
            decimal minTemperature,
            decimal maxTemperature,
            string temperatureUnit = "°C",
            bool requiresContinuousMonitoring = false,
            int? monitoringIntervalMinutes = null);

        Task<LoadTemperatureRequirement?> GetTemperatureRequirementAsync(long loadId);

        Task<LoadTemperatureRequirement> UpdateTemperatureRequirementAsync(
            long loadId,
            decimal minTemperature,
            decimal maxTemperature,
            string temperatureUnit = "°C",
            bool requiresContinuousMonitoring = false,
            int? monitoringIntervalMinutes = null);

        Task<bool> DeleteTemperatureRequirementAsync(long loadId);

        Task<bool> IsTemperatureInRangeAsync(long loadId, decimal currentTemperature);

        Task<IEnumerable<Load>> GetLoadsRequiringTemperatureMonitoringAsync();

        // This method would be called by a monitoring system or IoT devices
        Task<bool> RecordTemperatureReadingAsync(
            long loadId,
            decimal temperature,
            DateTime timestamp,
            string? deviceId = null);

        Task<IEnumerable<TemperatureReading>> GetTemperatureReadingsAsync(long loadId);
        
        Task<TemperatureReading?> GetTemperatureReadingByIdAsync(long id);
        
        Task<TemperatureReading?> GetLatestTemperatureReadingAsync(long loadId);
    }
}
