using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class BookingRequest
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Load ID must be greater than 0")]
        public long LoadId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Truck ID must be greater than 0")]
        public long TruckId { get; set; }

        public decimal? ProposedPrice { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; } // ISO Currency Code

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool ExpressBooking { get; set; } = false;

        public DateTime? RequestedPickupDate { get; set; }

        public DateTime? RequestedDeliveryDate { get; set; }
    }
}
