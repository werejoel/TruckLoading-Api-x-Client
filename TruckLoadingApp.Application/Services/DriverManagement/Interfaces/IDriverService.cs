using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverService
    {
        Task<bool> RegisterDriver(Driver driver, string userId);
        Task<bool> AssignDriverToTruck(long driverId, int truckId);
        Task<bool> UnassignDriver(long driverId);
        Task<IEnumerable<Driver>> GetAvailableDrivers();
        Task<IEnumerable<Driver>> GetDriversByCompany(string companyId);
        Task<bool> AssignOwnerAsDriver(string ownerId, int truckId);
        Task<bool> AssignDriverToTruckAsync(long driverId, long truckId);
        Task<bool> UnassignDriverAsync(long driverId);
        Task<IEnumerable<Driver>> GetAvailableDriversAsync();
        Task<IEnumerable<Driver>> GetDriversByCompanyAsync(string companyId);
        Task<bool> AssignOwnerAsDriverAsync(string ownerId, int truckId);
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByIdAsync(long id);
        Task<Driver> CreateDriverAsync(Driver driver);
        Task<bool> UpdateDriverAsync(Driver driver);
        Task<bool> DeleteDriverAsync(long id);
    }
}
