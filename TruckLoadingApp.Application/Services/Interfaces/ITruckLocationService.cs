using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Services
{
    public interface ITruckLocationService
    {
        Task UpdateTruckLocationAsync(int truckId, decimal latitude, decimal longitude);
        Task<TruckLocation?> GetCurrentTruckLocationAsync(long truckId);
        Task<List<object>?> GetTruckHistoryAsync(int truckId);
    }
} 