using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a route that a truck follows
    /// </summary>
    public class TruckRoute
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int TruckId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsRecurring { get; set; } = false;

        // Recurrence pattern (daily, weekly, etc.)
        public string? RecurrencePattern { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        [ForeignKey("TruckId")]
        public Truck Truck { get; set; } = null!;

        public ICollection<TruckRouteWaypoint> Waypoints { get; set; } = new List<TruckRouteWaypoint>();

        // Pricing information
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePricePerKm { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePricePerKg { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "USD";
    }
} 