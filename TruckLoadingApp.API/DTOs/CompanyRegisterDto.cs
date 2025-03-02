using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.DTOs
{
    public class CompanyRegisterDto
    {
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CompanyRegistrationNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ContactPersonFirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ContactPersonLastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
} 