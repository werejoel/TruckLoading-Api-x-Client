using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : IdentityUser
    {
        [Required]
        public UserType UserType { get; set; }

        [MaxLength(256)]
        public string? CompanyName { get; set; }

        // New fields for company registration
        public string? CompanyAddress { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? CompanyContact { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation property: A company can own multiple trucks
        public List<Truck>? Trucks { get; set; }
    }

}
