namespace TruckLoadingApp.Domain.Models
{
    public class TeamPerformanceReport
    {
        public long TeamId { get; set; }
        public DateRange Period { get; set; } = null!;
        public int TeamSize { get; set; }
        public decimal AverageDeliveries { get; set; }
        public decimal AverageRating { get; set; }
        public decimal AverageOnTimeDeliveryRate { get; set; }
        public decimal AverageSafetyScore { get; set; }
        public decimal AverageRestComplianceRate { get; set; }
        public int TotalSafetyIncidents { get; set; }
        public decimal SafetyIncidentsPerDriver { get; set; }
        public List<string> ImprovementAreas { get; set; } = new List<string>();
        public List<TopPerformer> TopPerformers { get; set; } = new List<TopPerformer>();
    }

    public class TopPerformer
    {
        public long DriverId { get; set; }
        public decimal Rating { get; set; }
        public int Deliveries { get; set; }
        public decimal OnTimeRate { get; set; }
        public virtual Driver Driver { get; set; } = null!;
    }

    public enum TrendDirection
    {
        Improving,
        Stable,
        Declining
    }
}
