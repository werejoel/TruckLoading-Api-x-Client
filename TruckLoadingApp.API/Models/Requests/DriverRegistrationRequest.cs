using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class DriverRegistrationRequest
    {
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public DateTime LicenseExpiryDate { get; set; }

        public int? Experience { get; set; }

        public decimal? SafetyRating { get; set; }
    }
}
