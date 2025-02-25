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

        [Required]
        public decimal LoadCapacityVolume { get; set; }

        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }

        [Required]
        public DateTime AvailabilityStartDate { get; set; }

        [Required]
        public DateTime AvailabilityEndDate { get; set; }
    }
}
