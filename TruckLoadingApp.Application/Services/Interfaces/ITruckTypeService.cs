using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckTypeService
    {
        Task<IEnumerable<TruckType>> GetAllTruckTypesAsync();
        Task<TruckType?> GetTruckTypeByIdAsync(int id);
        Task<IEnumerable<TruckCategory>> GetAllTruckCategoriesAsync();
        Task<TruckCategory?> GetTruckCategoryByIdAsync(int id);
        Task<IEnumerable<TruckType>> GetTruckTypesByCategoryIdAsync(int categoryId);
    }
} 