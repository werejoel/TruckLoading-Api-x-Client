using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Models.Requests
{
    public class AdminUpdateUserRequest
    {
        [MaxLength(256)]
        public string? CompanyName { get; set; }
        
        [MaxLength(256)]
        public string? CompanyAddress { get; set; }
        
        [MaxLength(50)]
        public string? CompanyRegistrationNumber { get; set; }
        
        [MaxLength(50)]
        public string? CompanyContact { get; set; }
    }
}
