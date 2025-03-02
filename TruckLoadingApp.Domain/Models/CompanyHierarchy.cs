using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class CompanyHierarchy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CompanyId { get; set; } = string.Empty;

        [Required]
        public string DepartmentName { get; set; } = string.Empty;

        public int? ParentDepartmentId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int Level { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public User Company { get; set; } = null!;

        [ForeignKey("ParentDepartmentId")]
        public CompanyHierarchy? ParentDepartment { get; set; }

        public ICollection<CompanyHierarchy> SubDepartments { get; set; } = new List<CompanyHierarchy>();
        public ICollection<DepartmentMember> DepartmentMembers { get; set; } = new List<DepartmentMember>();
    }

    public class DepartmentMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DepartmentRole Role { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public CompanyHierarchy Department { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public enum DepartmentRole
    {
        Employee,
        Supervisor,
        Manager,
        Director,
        VicePresident,
        President
    }
}
