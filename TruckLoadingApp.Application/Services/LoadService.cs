using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class LoadService : ILoadService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoadService> _logger;

        public LoadService(ApplicationDbContext context, ILogger<LoadService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateLoad(Load load)
        {
            _logger.LogInformation($"Creating a new load for Shipper ID: {load.ShipperId}");

            _context.Loads.Add(load);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Load created successfully");
            else
                _logger.LogWarning("Failed to create load");

            return result;
        }

        public async Task<Load?> GetLoadById(long loadId)
        {
            return await _context.Loads
                .Include(l => l.Shipper)
                .FirstOrDefaultAsync(l => l.Id == loadId);
        }

        public async Task<IEnumerable<Load>> GetAllLoads()
        {
            return await _context.Loads
                .Include(l => l.Shipper)
                .ToListAsync();
        }

        public async Task<bool> CancelLoad(long loadId)
        {
            var load = await _context.Loads.FindAsync(loadId);
            if (load == null)
                return false;

            _logger.LogInformation($"Canceling load ID: {loadId}");

            load.Status = LoadStatusEnum.Cancelled;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Load cancelled successfully");
            else
                _logger.LogWarning("Failed to cancel load");

            return result;
        }
    }
}
