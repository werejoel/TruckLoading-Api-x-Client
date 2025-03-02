namespace TruckLoadingApp.API.Models.Responses
{
    public class TruckRouteResponse
    {
        public long Id { get; set; }
        public long TruckId { get; set; }
        public string TruckRegistrationNumber { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; }
        public decimal BasePricePerKm { get; set; }
        public decimal BasePricePerKg { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<WaypointResponse> Waypoints { get; set; } = new List<WaypointResponse>();
    }

    public class WaypointResponse
    {
        public long Id { get; set; }
        public long TruckRouteId { get; set; }
        public int SequenceNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; }
        public int? StopDurationMinutes { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
} 