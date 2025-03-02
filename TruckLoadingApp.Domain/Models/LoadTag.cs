using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.Domain.Models
{
    public class LoadTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<LoadLoadTag> LoadTags { get; set; } = new List<LoadLoadTag>();
    }
}
