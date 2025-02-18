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
        /// <summary>
        /// Gets or sets the type of user.
        /// </summary>
        [Required]
        public UserType UserType { get; set; }

        /// <summary>
        /// Gets or sets the name of the company the user belongs to (nullable).
        /// </summary>
        [MaxLength(256)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the date and time the user was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the user was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
