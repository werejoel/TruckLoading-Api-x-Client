using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class ContactDetails
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
