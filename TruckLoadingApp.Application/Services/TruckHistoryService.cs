using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class TruckHistoryService : ITruckHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckHistoryService> _logger;

        public TruckHistoryService(ApplicationDbContext context, ILogger<TruckHistoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TruckHistory>> GetTruckHistoryAsync(long truckId)
        {
            return await _context.TruckHistory
                .Where(th => th.TruckId == truckId)
                .OrderByDescending(th => th.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> AddTruckHistoryEntryAsync(long truckId, string action, string? details = null, string? userId = null)
        {
            var truck = await _context.Trucks.FindAsync(truckId);
            if (truck == null)
            {
                _logger.LogWarning($"Attempted to add history for non-existent truck with ID {truckId}");
                return false;
            }

            var historyEntry = new TruckHistory
            {
                TruckId = (int)truckId,
                Action = action,
                Details = details,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            _context.TruckHistory.Add(historyEntry);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LogStatusChangeAsync(long truckId, TruckOperationalStatusEnum previousStatus, TruckOperationalStatusEnum newStatus, string? userId = null)
        {
            var truck = await _context.Trucks.FindAsync(truckId);
            if (truck == null)
            {
                _logger.LogWarning($"Attempted to log status change for non-existent truck with ID {truckId}");
                return false;
            }

            var historyEntry = new TruckHistory
            {
                TruckId = (int)truckId,
                Action = "Status Change",
                Details = $"Status changed from {previousStatus} to {newStatus}",
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                PreviousStatus = previousStatus,
                NewStatus = newStatus
            };

            _context.TruckHistory.Add(historyEntry);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LogDriverChangeAsync(long truckId, long? previousDriverId, long? newDriverId, string? userId = null)
        {
            var truck = await _context.Trucks.FindAsync(truckId);
            if (truck == null)
            {
                _logger.LogWarning($"Attempted to log driver change for non-existent truck with ID {truckId}");
                return false;
            }

            string details;
            if (previousDriverId == null && newDriverId != null)
            {
                details = $"Driver assigned (ID: {newDriverId})";
            }
            else if (previousDriverId != null && newDriverId == null)
            {
                details = $"Driver unassigned (Previous ID: {previousDriverId})";
            }
            else
            {
                details = $"Driver changed from ID {previousDriverId} to ID {newDriverId}";
            }

            var historyEntry = new TruckHistory
            {
                TruckId = (int)truckId,
                Action = "Driver Change",
                Details = details,
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                PreviousDriverId = previousDriverId,
                NewDriverId = newDriverId
            };

            _context.TruckHistory.Add(historyEntry);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 