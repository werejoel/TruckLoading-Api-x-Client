using Microsoft.AspNetCore.Identity;
using System;
using TruckLoadingApp.API.Enums;

namespace TruckLoadingApp.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TruckOwnerType? TruckOwnerType { get; set; }
        public string CompanyName { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
} 