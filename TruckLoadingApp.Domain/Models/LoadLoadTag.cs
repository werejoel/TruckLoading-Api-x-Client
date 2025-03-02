using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class LoadLoadTag
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long LoadId { get; set; }

        [Required]
        public int LoadTagId { get; set; }

        [ForeignKey("LoadId")]
        public Load Load { get; set; } = null!;

        [ForeignKey("LoadTagId")]
        public LoadTag LoadTag { get; set; } = null!;
    }
}
