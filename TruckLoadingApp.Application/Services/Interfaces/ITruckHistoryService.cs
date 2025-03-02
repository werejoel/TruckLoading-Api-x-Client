using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITruckHistoryService
    {
        Task<IEnumerable<TruckHistory>> GetTruckHistoryAsync(long truckId);
        Task<bool> AddTruckHistoryEntryAsync(long truckId, string action, string? details = null, string? userId = null);
        Task<bool> LogStatusChangeAsync(long truckId, TruckOperationalStatusEnum previousStatus, TruckOperationalStatusEnum newStatus, string? userId = null);
        Task<bool> LogDriverChangeAsync(long truckId, long? previousDriverId, long? newDriverId, string? userId = null);
    }
} 