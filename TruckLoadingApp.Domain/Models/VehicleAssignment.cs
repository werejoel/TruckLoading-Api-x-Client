namespace TruckLoadingApp.Domain.Models
{
    public class VehicleAssignment
    {
        public long Id { get; set; }
        public long DriverId { get; set; }
        public long TruckId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Driver Driver { get; set; } = null!;
        public virtual Truck Truck { get; set; } = null!;
    }
}
