using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class AdminUpdateUserRequest
    {
        [MaxLength(256)]
        public string? CompanyName { get; set; }
    }
}
