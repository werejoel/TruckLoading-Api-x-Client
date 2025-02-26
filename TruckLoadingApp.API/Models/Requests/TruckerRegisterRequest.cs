using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.API.Models.Requests
{
    public class TruckerRegisterRequest : RegisterRequest
    {
        public TruckerRegisterRequest()
        {
            
            UserType = UserType.Trucker;
        }

        [Required]
        public TruckOwnerType TruckOwnerType { get; set; }
    }
}
