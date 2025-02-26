using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;
namespace TruckLoadingApp.API.Models.Requests
{
    public class ShipperRegisterRequest : RegisterRequest
    {

        public ShipperRegisterRequest()
        {
            UserType = UserType.Shipper;
        }
    }
}
