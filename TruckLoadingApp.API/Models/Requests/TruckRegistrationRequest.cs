using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class TruckRegistrationRequest
    {
        [Required]
        public int TruckTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NumberPlate { get; set; } = string.Empty;

        [Required]
        public decimal LoadCapacityWeight { get; set; }
        public decimal? Height { get; set; } // ✅ Add back if needed
        public decimal? Width { get; set; }  // ✅ Add back if needed
        public decimal? Length { get; set; } // ✅ Add back if needed

        [Required]
        public decimal LoadCapacityVolume { get; set; }

        [Required]
        public DateTime AvailabilityStartDate { get; set; }

        [Required]
        public DateTime AvailabilityEndDate { get; set; }

        [MaxLength(256)]
        public string? PreferredRoute { get; set; }

        [MaxLength(256)]
        public string? DriverName { get; set; }

        [MaxLength(256)]
        public string? DriverContactInformation { get; set; }

        [MaxLength(256)]
        public string? InsuranceInformation { get; set; }
    }
}
