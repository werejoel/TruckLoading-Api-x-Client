using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class SearchTrucksRequest
    {
        [Required]
        public decimal OriginLatitude { get; set; }

        [Required]
        public decimal OriginLongitude { get; set; }

        [Required]
        public decimal DestinationLatitude { get; set; }

        [Required]
        public decimal DestinationLongitude { get; set; }

        [Required]
        public decimal Weight { get; set; }

        public decimal? Height { get; set; }

        public decimal? Width { get; set; }

        public decimal? Length { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
    }
}
