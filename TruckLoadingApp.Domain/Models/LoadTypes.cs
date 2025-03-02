using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.Domain.Models
{
    public class LoadType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool HasSpecialHandling { get; set; }

        public bool IsHazardous { get; set; }

        public bool RequiresRefrigeration { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
