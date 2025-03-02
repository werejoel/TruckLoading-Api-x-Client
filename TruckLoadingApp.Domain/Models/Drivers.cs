using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class Driver
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public DateTime LicenseExpiryDate { get; set; }

        public int? Experience { get; set; }
        public decimal? SafetyRating { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int? TruckId { get; set; }

        // Remove the ForeignKey attribute to avoid duplicate relationships
        public Truck? Truck { get; set; }

        // Navigation Properties
        public virtual ICollection<DriverDocument> Documents { get; set; } = new List<DriverDocument>();
        public virtual ICollection<DriverCertification> Certifications { get; set; } = new List<DriverCertification>();
        public virtual ICollection<DriverPerformance> Performances { get; set; } = new List<DriverPerformance>();
        public virtual ICollection<DriverSchedule> Schedules { get; set; } = new List<DriverSchedule>();
        public virtual ICollection<DriverRestPeriod> RestPeriods { get; set; } = new List<DriverRestPeriod>();
        public virtual DriverRoutePreference? RoutePreferences { get; set; }
    }
}
