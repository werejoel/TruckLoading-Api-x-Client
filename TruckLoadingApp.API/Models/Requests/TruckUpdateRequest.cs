using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class TruckUpdateRequest
    {
        [Required]
        public int TruckTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NumberPlate { get; set; } = string.Empty;

        [Required]
        public decimal LoadCapacityWeight { get; set; }

        [Required]
        public decimal LoadCapacityVolume { get; set; }

        [Required]
        public DateTime AvailabilityStartDate { get; set; }

        [Required]
        public DateTime AvailabilityEndDate { get; set; }

        [MaxLength(256)]
        public string? PreferredRoute { get; set; }

        public bool IsApproved { get; set; }
    }
}
