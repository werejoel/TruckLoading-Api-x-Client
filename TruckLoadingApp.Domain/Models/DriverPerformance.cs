using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class DriverPerformance
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Safety Metrics
        public int SafetyIncidents { get; set; }
        public int SafetyViolations { get; set; }
        public int SpeedingEvents { get; set; }
        public int HarshBrakingEvents { get; set; }
        public int HarshAccelerationEvents { get; set; }

        // Performance Metrics
        public int OnTimeDeliveries { get; set; }
        public int LateDeliveries { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal CustomerRating { get; set; }
        public decimal FuelEfficiency { get; set; } // Miles per gallon or km per liter

        // Time Management
        public TimeSpan TotalDrivingTime { get; set; }
        public TimeSpan TotalRestTime { get; set; }
        public int RestBreakViolations { get; set; }
        public int HoursOfServiceViolations { get; set; }

        // Maintenance
        public int VehicleInspectionScore { get; set; }
        public int MaintenanceIssuesReported { get; set; }

        // Overall Performance
        [Required]
        [Range(0, 100)]
        public decimal OverallPerformanceScore { get; set; }
        [Required]
        [Range(0, 100)]
        public decimal Rating { get; set; }
        public string? PerformanceNotes { get; set; }

        
        public virtual Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public decimal SafetyScore { get; set; }
    }
}
