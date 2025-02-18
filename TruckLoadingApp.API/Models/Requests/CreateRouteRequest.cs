using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class CreateRouteRequest
    {
        [Required]
        public int TruckId { get; set; }

        [Required]
        public string RouteName { get; set; } = string.Empty;
    }
}
