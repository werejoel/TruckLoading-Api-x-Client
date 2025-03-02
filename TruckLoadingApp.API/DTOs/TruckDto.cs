namespace TruckLoadingApp.API.DTOs
{
    public class TruckDto
    {
        public string Id { get; set; }
        public string NumberPlate { get; set; }
        public string TruckType { get; set; }
        public double LoadCapacityWeight { get; set; }
        public bool IsApproved { get; set; }
        public string OperationalStatus { get; set; }
    }
} 