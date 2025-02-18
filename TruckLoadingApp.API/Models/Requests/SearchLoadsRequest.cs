using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class SearchLoadsRequest
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
        public decimal MaxWeight { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
    }
}
