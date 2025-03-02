using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents an instance of a recurring schedule
    /// </summary>
    public class RecurringScheduleInstance
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public long ParentScheduleId { get; set; }
        
        [ForeignKey("ParentScheduleId")]
        public DriverSchedule ParentSchedule { get; set; } = null!;
        
        [Required]
        public long DriverId { get; set; }
        
        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;
        
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
        
        // Instance number in the recurring series
        public int InstanceNumber { get; set; }
        
        // Track if this instance has been modified from the original pattern
        public bool IsModified { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
} 