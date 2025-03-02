namespace TruckLoadingApp.Domain.Models
{
    public enum TrainingStatus
    {
        Pending,
        Scheduled,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }

    public enum CertificationStatus
    {
        Active,
        Expired,
        Suspended,
        Revoked,
        Pending
    }

    public enum TrainingPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public static class RestRegulations
    {
        public const int DaysBetweenRequiredRests = 7; // Standard regulation for required rest periods
        public const int MinimumRestHours = 8; // Minimum required rest hours
        public const int MaxDrivingHoursPerDay = 10; // Maximum allowed driving hours per day

        public const int MinimumDailyRest = 11; // Minimum daily rest in hours
        public const int MinimumWeeklyRest = 45; // Minimum weekly rest in hours
        public const int MaximumDrivingPeriod = 9; // Maximum driving period in hours
        public const int MinimumBreakDuration = 45; // Minimum break duration in minutes
        public const int RequiredBreakAfter = 4; // Required break after driving hours
    }

    public class ImprovementArea
    {
        public string Area { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new List<string>();
        public int Priority { get; set; }
        public decimal CurrentScore { get; set; }
        public decimal TargetScore { get; set; }
    }
}
