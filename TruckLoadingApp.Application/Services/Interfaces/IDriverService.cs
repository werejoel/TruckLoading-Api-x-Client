
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IDriverService
    {
        Task<bool> RegisterDriver(Driver driver, string userId);
        Task<bool> AssignDriverToTruck(long driverId, int truckId);
        Task<bool> UnassignDriver(long driverId);
        Task<IEnumerable<Driver>> GetAvailableDrivers();
        Task<IEnumerable<Driver>> GetDriversByCompany(string companyId);
        Task<bool> AssignOwnerAsDriver(string ownerId, int truckId);
    }
}
