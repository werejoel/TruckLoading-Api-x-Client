

using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ILoadService
    {
        Task<bool> CreateLoad(Load load);
        Task<Load?> GetLoadById(long loadId);
        Task<IEnumerable<Load>> GetAllLoads();
        Task<bool> CancelLoad(long loadId);
    }
}
