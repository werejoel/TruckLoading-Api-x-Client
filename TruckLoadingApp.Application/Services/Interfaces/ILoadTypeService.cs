using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ILoadTypeService
    {
        Task<IEnumerable<LoadType>> GetAllLoadTypesAsync();
        Task<LoadType?> GetLoadTypeByIdAsync(int id);
        Task<LoadType> CreateLoadTypeAsync(LoadType loadType);
        Task<bool> UpdateLoadTypeAsync(LoadType loadType);
        Task<bool> DeleteLoadTypeAsync(int id);
    }
} 