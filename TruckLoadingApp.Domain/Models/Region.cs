using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.Domain.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
        //Could be coordinates or zip code
        public string? Coordinates { get; set; }
    }
}
