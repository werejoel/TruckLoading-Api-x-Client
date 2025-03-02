using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public string CompanyId { get; set; } = string.Empty;

        [Required]
        public string TeamLeaderId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public User Company { get; set; } = null!;

        [ForeignKey("TeamLeaderId")]
        public User TeamLeader { get; set; } = null!;

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}
