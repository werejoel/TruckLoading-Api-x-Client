namespace TruckLoadingApp.API.Models.DTOs
{
    public class TruckDto
    {
        public int Id { get; set; }
        public string NumberPlate { get; set; } = string.Empty;
        public string TruckType { get; set; } = string.Empty;
        public decimal LoadCapacityWeight { get; set; }
        public bool IsApproved { get; set; }
        public string OperationalStatus { get; set; } = string.Empty;
    }
} 