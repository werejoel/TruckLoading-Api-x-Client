using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        // Company-specific fields
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? CompanyContact { get; set; }

        // Trucker-specific fields
        public TruckOwnerType? TruckOwnerType { get; set; }
    }

    public class ChangeRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string NewRole { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public TruckOwnerType? TruckOwnerType { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
    }
} 