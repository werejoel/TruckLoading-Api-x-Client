using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    public class Ratings
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long BookingId { get; set; }

        [Required]
        public string RatedByUserId { get; set; } = string.Empty;

        [Required]
        public string RatedUserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!; // Navigation property

        [ForeignKey("RatedByUserId")]
        public User RatedByUser { get; set; } = null!; // Navigation property

        [ForeignKey("RatedUserId")]
        public User RatedUser { get; set; } = null!; // Navigation property
        public DateTime? RatingDate { get; set; }
        public int? ReportReasonId { get; set; }
    }
}
