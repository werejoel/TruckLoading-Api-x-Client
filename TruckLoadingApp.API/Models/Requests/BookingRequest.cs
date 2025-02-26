using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class BookingRequest
    {
        [Required]
        public long LoadId { get; set; }

        [Required]
        public int TruckId { get; set; }
    }
}
