using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
