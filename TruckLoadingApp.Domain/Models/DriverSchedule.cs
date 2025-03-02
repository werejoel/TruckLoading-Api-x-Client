using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class DriverSchedule
    {
        public DriverSchedule()
        {
            RecurringInstances = new List<DriverSchedule>();
        }
        
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public ScheduleStatus Status { get; set; }

        public string? Notes { get; set; }

        // Optional load assignment
        public long? LoadId { get; set; }

        [ForeignKey("LoadId")]
        public Load? Load { get; set; }


        public Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // For recurring schedules
        public bool IsRecurring { get; set; }
        public RecurrencePattern? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        
        // For recurring schedule instances
        public long? RecurringScheduleId { get; set; }
        
        [ForeignKey("RecurringScheduleId")]
        public DriverSchedule? RecurringScheduleParent { get; set; }
        
        public int? InstanceNumber { get; set; }
        
        // Collection of instances for a recurring schedule
        [InverseProperty("RecurringScheduleParent")]
        public virtual ICollection<DriverSchedule> RecurringInstances { get; set; }

        // Distance tracking
        public decimal DistanceCovered { get; set; }

        // Fuel tracking
        public decimal FuelUsed { get; set; }
    }

    public enum ScheduleStatus
    {
        Pending,
        Confirmed,
        InProgress,
        Completed,
        Cancelled
    }

    public enum RecurrencePattern
    {
        Daily,
        Weekly,
        BiWeekly,
        Monthly
    }
}
