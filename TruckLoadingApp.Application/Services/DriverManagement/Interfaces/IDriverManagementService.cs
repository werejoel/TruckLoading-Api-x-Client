using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverManagementService
    {

        // Analytics
        Task<DriverPerformanceAnalytics> GetDriverAnalyticsAsync(long driverId, DateTime startDate, DateTime endDate);
        Task<TeamPerformanceReport> GetTeamPerformanceReportAsync(IEnumerable<long> driverIds, DateTime startDate, DateTime endDate);

    }

    public class PerformanceMetrics
    {
        public decimal SafetyScore { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public decimal FuelEfficiency { get; set; }
        public int ComplianceViolations { get; set; }
        public TimeSpan TotalDrivingTime { get; set; }
        public decimal MaintenanceScore { get; set; }
        public decimal OverallScore { get; set; }
    }

    public class RestComplianceStatus
    {
        public bool IsCompliant { get; set; }
        public TimeSpan TotalDrivingTime { get; set; }
        public TimeSpan TotalRestTime { get; set; }
        public int Violations { get; set; }
        public string? ComplianceNotes { get; set; }
        public DateTime? NextRequiredRest { get; set; }
    }

    public class RouteRequirements
    {
        public string? Region { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? LoadWeight { get; set; }
        public string? LoadType { get; set; }
        public bool RequireNightDriving { get; set; }
        public bool HasSevereWeather { get; set; }
    }
}
