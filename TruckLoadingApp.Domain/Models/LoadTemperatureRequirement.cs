using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class LoadTemperatureRequirement
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long LoadId { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MinTemperature { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxTemperature { get; set; }

        [Required]
        [MaxLength(10)]
        public string TemperatureUnit { get; set; } = "°C";

        public bool RequiresContinuousMonitoring { get; set; }

        public int? MonitoringIntervalMinutes { get; set; }

        [ForeignKey("LoadId")]
        public Load Load { get; set; } = null!;
    }
}
