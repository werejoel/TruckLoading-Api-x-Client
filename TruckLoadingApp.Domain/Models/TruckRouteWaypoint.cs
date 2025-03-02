using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a waypoint in a truck route
    /// </summary>
    public class TruckRouteWaypoint
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long TruckRouteId { get; set; }

        [Required]
        public int SequenceNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        // Optional estimated arrival time
        public DateTime? EstimatedArrivalTime { get; set; }

        // Optional stop duration in minutes
        public int? StopDurationMinutes { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        [ForeignKey("TruckRouteId")]
        public TruckRoute TruckRoute { get; set; } = null!;
    }
} 