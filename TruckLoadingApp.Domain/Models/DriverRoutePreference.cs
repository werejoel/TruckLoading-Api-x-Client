using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class DriverRoutePreference
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DriverId { get; set; }

        // Geographic preferences
        public string? PreferredRegions { get; set; }
        public string? AvoidedRegions { get; set; }

        // Route type preferences
        public bool PreferHighways { get; set; }
        public bool AvoidTolls { get; set; }
        public bool AvoidFerries { get; set; }
        public bool AvoidUrbanCenters { get; set; }

        // Time preferences
        public TimeSpan? PreferredStartTime { get; set; }
        public TimeSpan? PreferredEndTime { get; set; }
        public bool AvoidNightDriving { get; set; }
        public bool AvoidPeakHours { get; set; }

        // Load preferences
        public decimal? MaxPreferredWeight { get; set; }
        public string? PreferredLoadTypes { get; set; }
        public string? AvoidedLoadTypes { get; set; }

        // Weather preferences
        public bool AvoidSevereWeather { get; set; }
        public decimal? MaxWindSpeed { get; set; }
        public bool AvoidSnowRoutes { get; set; }

        // Rest stop preferences
        public int PreferredRestStopInterval { get; set; } // In minutes
        public string? PreferredRestStopAmenities { get; set; }

        // Priority settings (1-5, where 5 is highest priority)
        public int TimePriority { get; set; }
        public int SafetyPriority { get; set; }
        public int FuelEfficiencyPriority { get; set; }
        public int ComfortPriority { get; set; }

        [MaxLength(500)]
        public string? AdditionalNotes { get; set; }

       
        public Driver Driver { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
