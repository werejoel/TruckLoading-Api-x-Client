namespace TruckLoadingApp.Domain.Models
{
    public class DriverPerformanceAnalytics
    {
        public long DriverId { get; set; }
        public DateRange Period { get; set; } = null!;
        public int TotalTrips { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal TotalDistanceCovered { get; set; }
        public decimal AverageFuelEfficiency { get; set; }
        public TimeSpan TotalDrivingTime { get; set; }
        public decimal TotalDrivingHours => (decimal)TotalDrivingTime.TotalHours;
        public decimal OnTimeDeliveryRate { get; set; }
        public decimal AverageSafetyScore { get; set; }
        public decimal SafetyScore { get; set; }
        public decimal AverageRating { get; set; }
        public int RestViolations { get; set; }
        public decimal CustomerSatisfactionScore { get; set; }
        public decimal MaintenanceScore { get; set; }
        public decimal OverallPerformanceScore { get; set; }
        public PerformanceTrend PerformanceTrend { get; set; }
        public int SafetyIncidents { get; set; }
        public decimal RestComplianceRate { get; set; }
        public decimal RoutePreferenceAlignment { get; set; }
        public decimal RoutePreferenceCompliance { get; set; }
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public enum PerformanceTrend
    {
        Improving,
        Stable,
        Declining
    }
}
