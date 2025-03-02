using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public TeamRole Role { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TeamId")]
        public Team Team { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public enum TeamRole
    {
        Member,
        Supervisor,
        Dispatcher,
        Administrator
    }
}
