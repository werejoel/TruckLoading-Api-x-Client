using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    public class Driver
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public DateTime LicenseExpiryDate { get; set; }

        public int? Experience { get; set; }
        public decimal? SafetyRating { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // 🚀 Driver may or may not have an assigned truck
        public int? TruckId { get; set; }
        public Truck? Truck { get; set; }
    }
}
