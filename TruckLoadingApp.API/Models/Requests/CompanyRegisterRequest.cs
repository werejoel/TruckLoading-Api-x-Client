using System.ComponentModel.DataAnnotations;

using TruckLoadingApp.Domain.Enums;
namespace TruckLoadingApp.API.Models.Requests
{
    public class CompanyRegisterRequest : RegisterRequest
    {
        public CompanyRegisterRequest()
        {
            UserType = UserType.Company;
        }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string CompanyAddress { get; set; } = string.Empty;

        [Required]
        public string CompanyRegistrationNumber { get; set; } = string.Empty;

        [Required]
        public string CompanyContact { get; set; } = string.Empty;
    }
}
