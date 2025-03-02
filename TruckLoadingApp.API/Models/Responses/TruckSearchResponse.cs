using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.API.Models.Responses
{
    public class TruckSearchResponse
    {
        public long Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public int TruckTypeId { get; set; }
        public string TruckTypeName { get; set; } = string.Empty;
        public decimal AvailableCapacityWeight { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal VolumeCapacity { get; set; }
        public bool HasRefrigeration { get; set; }
        public bool HasLiftgate { get; set; }
        public bool HasLoadingRamp { get; set; }
        public bool CanTransportHazardousMaterials { get; set; }
        public string? HazardousMaterialsClasses { get; set; }
        public DateTime AvailabilityStartDate { get; set; }
        public DateTime AvailabilityEndDate { get; set; }
        public TruckOperationalStatusEnum OperationalStatus { get; set; }
        
        // Driver information
        public string? DriverName { get; set; }
        public string? DriverPhoneNumber { get; set; }
        public double? DriverRating { get; set; }
        
        // Company information
        public string? CompanyName { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public double? CompanyRating { get; set; }
        
        // Location and route information
        public decimal? CurrentLatitude { get; set; }
        public decimal? CurrentLongitude { get; set; }
        public double? DistanceToPickup { get; set; } // in kilometers
        public string? EstimatedTimeToPickup { get; set; }
        public double? RouteMatchPercentage { get; set; }
        
        // Pricing information
        public decimal? EstimatedPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public string? PricingModel { get; set; }
    }
} 