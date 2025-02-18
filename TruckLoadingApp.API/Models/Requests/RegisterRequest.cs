using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Models.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        [MaxLength(256)]
        public string? CompanyName { get; set; }
    }
}
