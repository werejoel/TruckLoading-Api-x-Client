using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class TemperatureReading
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long LoadId { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Temperature { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [MaxLength(50)]
        public string? DeviceId { get; set; }

        public bool IsWithinRange { get; set; }

        [ForeignKey("LoadId")]
        public Load Load { get; set; } = null!;
    }
}
