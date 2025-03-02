using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    public class TruckHistory
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int TruckId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Details { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("TruckId")]
        public Truck Truck { get; set; } = null!;

        public TruckOperationalStatusEnum? PreviousStatus { get; set; }
        public TruckOperationalStatusEnum? NewStatus { get; set; }

        public long? PreviousDriverId { get; set; }
        public long? NewDriverId { get; set; }
    }
} 