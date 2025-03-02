using System;
using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.API.Enums;

namespace TruckLoadingApp.API.Models
{
    public class Trucker
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        public TruckOwnerType TruckOwnerType { get; set; }
        
        public string CompanyName { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
} 