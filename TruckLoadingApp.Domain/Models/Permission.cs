using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.Domain.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public int PermissionId { get; set; }

        public Permission Permission { get; set; } = null!;
    }

    public static class Permissions
    {
        // Load Management
        public const string ViewLoads = "loads.view";
        public const string CreateLoads = "loads.create";
        public const string UpdateLoads = "loads.update";
        public const string DeleteLoads = "loads.delete";
        public const string AssignLoads = "loads.assign";

        // Team Management
        public const string ViewTeams = "teams.view";
        public const string CreateTeams = "teams.create";
        public const string UpdateTeams = "teams.update";
        public const string DeleteTeams = "teams.delete";
        public const string ManageTeamMembers = "teams.manage_members";

        // User Management
        public const string ViewUsers = "users.view";
        public const string CreateUsers = "users.create";
        public const string UpdateUsers = "users.update";
        public const string DeleteUsers = "users.delete";
        public const string ManageUserRoles = "users.manage_roles";

        // Truck Management
        public const string ViewTrucks = "trucks.view";
        public const string CreateTrucks = "trucks.create";
        public const string UpdateTrucks = "trucks.update";
        public const string DeleteTrucks = "trucks.delete";
        public const string AssignDrivers = "trucks.assign_drivers";

        // Driver Management
        public const string ViewDrivers = "drivers.view";
        public const string CreateDrivers = "drivers.create";
        public const string UpdateDrivers = "drivers.update";
        public const string DeleteDrivers = "drivers.delete";
        public const string ManageDriverSchedules = "drivers.manage_schedules";

        // Company Management
        public const string ViewCompany = "company.view";
        public const string UpdateCompany = "company.update";
        public const string ManageCompanySettings = "company.manage_settings";

        // Reports
        public const string ViewReports = "reports.view";
        public const string CreateReports = "reports.create";
        public const string ExportReports = "reports.export";

        // System Settings
        public const string ViewSettings = "settings.view";
        public const string UpdateSettings = "settings.update";
    }
}
