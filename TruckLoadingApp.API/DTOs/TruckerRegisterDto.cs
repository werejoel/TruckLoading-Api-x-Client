using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.API.DTOs
{
    public class TruckerRegisterDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public TruckOwnerType TruckOwnerType { get; set; }
        
        // Driver-specific fields
        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }
        
        [Required]
        public DateTime LicenseExpiryDate { get; set; }
        
        public int? Experience { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }
    }
} 