using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        // Truck Owner Type (Only for Truckers)
        public TruckOwnerType? TruckOwnerType { get; set; }

        // Company-specific fields (Only for Company users)
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? CompanyContact { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Authentication-related properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // One-to-One Relationship with ContactDetails
        public ContactDetails? ContactDetails { get; set; }

        // One-to-Many Relationship: A Trucker or Company can have multiple Trucks
        public List<Truck>? Trucks { get; set; }
    }
}
