using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class SearchTrucksRequest
    {
        [Required]
        [Range(-90, 90)]
        public decimal OriginLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal OriginLongitude { get; set; }

        [Required]
        [Range(-90, 90)]
        public decimal DestinationLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal DestinationLongitude { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal Weight { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
        public decimal? Height { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
        public decimal? Width { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public decimal? Length { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        public int? RequiredTruckTypeId { get; set; }

        public bool RequiresTemperatureControl { get; set; }

        public bool RequiresHazardousMaterialHandling { get; set; }

        [Range(0, 500, ErrorMessage = "Maximum search radius is 500 km")]
        public int? SearchRadiusKm { get; set; }
    }
}
