using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class AddWaypointRequest
    {
        [Required]
        public long RouteId { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public int StopOrder { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }
    }
}
