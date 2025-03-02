using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class TruckRouteCreateRequest
    {
        [Required]
        public long TruckId { get; set; }

        [Required]
        [StringLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsRecurring { get; set; } = false;

        [StringLength(50)]
        public string? RecurrencePattern { get; set; }

        [Required]
        [Range(0.01, 1000)]
        public decimal BasePricePerKm { get; set; }

        [Required]
        [Range(0.01, 1000)]
        public decimal BasePricePerKg { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        public List<WaypointCreateRequest> Waypoints { get; set; } = new List<WaypointCreateRequest>();
    }

    public class WaypointCreateRequest
    {
        [Required]
        public int SequenceNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        public DateTime? EstimatedArrivalTime { get; set; }

        [Range(0, 1440)]
        public int? StopDurationMinutes { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
} 