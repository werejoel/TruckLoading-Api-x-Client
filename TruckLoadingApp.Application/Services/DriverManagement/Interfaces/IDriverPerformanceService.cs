using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverPerformanceService
    {
        Task<DriverPerformance> RecordPerformanceAsync(DriverPerformance performance);
        Task<DriverPerformance?> GetPerformanceByIdAsync(long performanceId);
        Task<IEnumerable<DriverPerformance>> GetDriverPerformanceHistoryAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<decimal> CalculateDriverRatingAsync(long driverId);
        Task<PerformanceMetrics> GetDriverMetricsAsync(long driverId, DateTime startDate, DateTime endDate);
        decimal CalculateSafetyScore(List<DriverPerformance> performances);
    }
}
