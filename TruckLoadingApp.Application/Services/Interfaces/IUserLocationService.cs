
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IUserLocationService
    {
        Task<UserLocation?> GetUserLocation(string userId);
    }
}
