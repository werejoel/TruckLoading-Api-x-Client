using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.API.Models.Requests
{
    public class UserProfileUpdateRequest
    {
        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? CompanyName { get; set; }

        public TruckOwnerType? TruckOwnerType { get; set; }
    }
}
