using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class UserActivity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ActivityType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? EntityType { get; set; }

        public string? EntityId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        // Additional metadata stored as JSON
        public string? Metadata { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

    }

   public static class ActivityTypes
    {
        public const string AssignTraining = "AssignTraining";
        public const string UpdateTraining = "UpdateTraining";
        public const string VerifyDocument = "VerifyDocument";
        public const string UpdateCertification = "UpdateCertification";
        public const string AssignVehicle = "AssignVehicle";
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string CreateLoad = "CREATE_LOAD";
        public const string UpdateLoad = "UPDATE_LOAD";
        public const string DeleteLoad = "DELETE_LOAD";
        public const string AssignLoad = "ASSIGN_LOAD";
        public const string CreateTeam = "CREATE_TEAM";
        public const string UpdateTeam = "UPDATE_TEAM";
        public const string DeleteTeam = "DELETE_TEAM";
        public const string AddTeamMember = "ADD_TEAM_MEMBER";
        public const string RemoveTeamMember = "REMOVE_TEAM_MEMBER";
        public const string UpdateTeamMember = "UPDATE_TEAM_MEMBER";
        public const string UpdateUserProfile = "UPDATE_PROFILE";
        public const string ChangePassword = "CHANGE_PASSWORD";
        public const string CreateTruck = "CREATE_TRUCK";
        public const string UpdateTruck = "UPDATE_TRUCK";
        public const string DeleteTruck = "DELETE_TRUCK";
        public const string AssignDriver = "ASSIGN_DRIVER";
        public const string UpdatePermissions = "UPDATE_PERMISSIONS";
        public const string CreateDepartment = "CREATE_DEPARTMENT";
        public const string UpdateDepartment = "UPDATE_DEPARTMENT";
        public const string DeleteDepartment = "DELETE_DEPARTMENT";
        public const string AddDepartmentMember = "ADD_DEPARTMENT_MEMBER";
        public const string RemoveDepartmentMember = "REMOVE_DEPARTMENT_MEMBER";
        public const string UpdateDepartmentMember = "UPDATE_DEPARTMENT_MEMBER";
        public const string SendMessage = "SEND_MESSAGE";
        public const string ReadMessage = "READ_MESSAGE";
        public const string DeleteMessage = "DELETE_MESSAGE";
        public const string CreateSchedule = "CREATE_SCHEDULE";
        public const string UpdateSchedule = "UPDATE_SCHEDULE";
        public const string DeleteSchedule = "DELETE_SCHEDULE";
        public const string RecordPerformance = "RECORD_PERFORMANCE";
        public const string UpdateDocument = "UPDATE_DOCUMENT";
        public const string DeleteDocument = "DELETE_DOCUMENT";
        public const string UpdateRestPeriod = "UPDATE_REST_PERIOD";
        public const string DeleteRestPeriod = "DELETE_REST_PERIOD";
        public const string RecordRestPeriod = "RECORD_REST_PERIOD";
        public const string AddDocument = "ADD_DOCUMENT";
        public const string UpdateRoutePreferences = "UPDATE_ROUTE_PREFERENCES";
    }
}
