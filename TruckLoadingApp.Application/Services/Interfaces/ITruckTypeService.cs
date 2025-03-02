using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckTypeService
    {
        Task<IEnumerable<TruckType>> GetAllTruckTypesAsync();
        Task<TruckType?> GetTruckTypeByIdAsync(int id);
        Task<TruckType> CreateTruckTypeAsync(TruckType truckType);
        Task<bool> UpdateTruckTypeAsync(TruckType truckType);
        Task<bool> DeleteTruckTypeAsync(int id);
    }
} 