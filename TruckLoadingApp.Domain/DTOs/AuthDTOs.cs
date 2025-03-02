using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Base registration DTO with common fields
    public abstract class BaseRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }
    }

    // Shipper-specific registration DTO
    public class ShipperRegisterDto : BaseRegisterDto
    {
        public ShipperRegisterDto()
        {
            UserType = UserType.Shipper;
        }
    }

    // Trucker-specific registration DTO
    public class TruckerRegisterDto : BaseRegisterDto
    {
        public TruckerRegisterDto()
        {
            UserType = UserType.Trucker;
        }

        [Required]
        public TruckOwnerType TruckOwnerType { get; set; }
    }

    // Company-specific registration DTO
    public class CompanyRegisterDto : BaseRegisterDto
    {
        public CompanyRegisterDto()
        {
            UserType = UserType.Company;
        }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string CompanyAddress { get; set; } = string.Empty;

        [Required]
        public string CompanyRegistrationNumber { get; set; } = string.Empty;

        [Required]
        public string CompanyContact { get; set; } = string.Empty;
    }

    // Admin-specific registration DTO
    public class AdminRegisterDto : BaseRegisterDto
    {
        public AdminRegisterDto()
        {
            UserType = UserType.Admin;
        }
    }

    // Legacy DTO for backward compatibility
    public class RegisterDto : BaseRegisterDto
    {
        // Optional fields based on user type
        public TruckOwnerType? TruckOwnerType { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? CompanyContact { get; set; }
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
} 