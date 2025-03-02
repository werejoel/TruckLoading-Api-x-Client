using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class DriverRestPeriod
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public RestType Type { get; set; }

        public RestStatus Status { get; set; }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        // For compliance tracking
        public bool IsCompliant { get; set; }
        public string? ComplianceNotes { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Calculated properties
        [NotMapped]
        public TimeSpan Duration => EndTime - StartTime;
    }

    public enum RestType
    {
        ShortBreak,      // Short rest periods during the day
        DailyRest,       // Main daily rest period
        WeeklyRest,      // Weekly rest period
        CompensatoryRest // Make-up rest for reduced rest periods
    }

    public enum RestStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Interrupted,
        Skipped
    }
}
