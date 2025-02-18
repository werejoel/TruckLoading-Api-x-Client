using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class UpdateLocationRequest
    {
        [Required]
        public int TruckId { get; set; }

        [Required]
        public decimal CurrentLatitude { get; set; }

        [Required]
        public decimal CurrentLongitude { get; set; }
    }
}
