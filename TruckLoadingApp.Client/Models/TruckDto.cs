namespace TruckLoadingApp.Client.Models
{
    public class TruckDto
    {
        public int Id { get; set; }
        public int TruckTypeId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string NumberPlate { get; set; } = string.Empty;
        public decimal LoadCapacityWeight { get; set; }
        public decimal LoadCapacityVolume { get; set; }
        public decimal? CurrentLatitude { get; set; }
        public decimal? CurrentLongitude { get; set; }
        public DateTime AvailabilityStartDate { get; set; }
        public DateTime AvailabilityEndDate { get; set; }
        public string? PreferredRoute { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContactInformation { get; set; }
        public string? InsuranceInformation { get; set; }
        public bool IsApproved { get; set; }
    }
}
