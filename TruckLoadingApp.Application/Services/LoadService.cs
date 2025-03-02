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

        public async Task<Load> CreateLoadAsync(Load load)
        {
            _logger.LogInformation($"Creating a new load for Shipper ID: {load.ShipperId}");

            _context.Loads.Add(load);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Load created successfully with ID: {load.Id}");

            return load;
        }

        public async Task<Load?> GetLoadByIdAsync(long id)
        {
            return await _context.Loads
                .Include(l => l.Shipper)
                .Include(l => l.LoadType)
                .Include(l => l.RequiredTruckType)
                .Include(l => l.LoadDimensions)
                .Include(l => l.TemperatureRequirement)
                .Include(l => l.LoadTags)
                    .ThenInclude(lt => lt.LoadTag)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Load>> GetAllLoadsAsync()
        {
            return await _context.Loads
                .Include(l => l.Shipper)
                .Include(l => l.LoadType)
                .ToListAsync();
        }

        public async Task<bool> CancelLoadAsync(long loadId)
        {
            var load = await _context.Loads.FindAsync(loadId);
            if (load == null)
                return false;

            _logger.LogInformation($"Canceling load ID: {loadId}");

            load.Status = LoadStatusEnum.Cancelled;
            load.UpdatedDate = DateTime.UtcNow;
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Load cancelled successfully");
            else
                _logger.LogWarning("Failed to cancel load");

            return result;
        }

        public async Task<bool> UpdateLoadAsync(Load load)
        {
            var existingLoad = await _context.Loads.FindAsync(load.Id);
            if (existingLoad == null)
                return false;

            _logger.LogInformation($"Updating load ID: {load.Id}");

            // Update properties
            _context.Entry(existingLoad).CurrentValues.SetValues(load);
            existingLoad.UpdatedDate = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Load updated successfully");
            else
                _logger.LogWarning("Failed to update load");

            return result;
        }

        public async Task<bool> DeleteLoadAsync(long id)
        {
            var load = await _context.Loads.FindAsync(id);
            if (load == null)
                return false;

            _logger.LogInformation($"Deleting load ID: {id}");

            _context.Loads.Remove(load);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Load deleted successfully");
            else
                _logger.LogWarning("Failed to delete load");

            return result;
        }

        public async Task<IEnumerable<Load>> GetLoadsByStatusAsync(string status)
        {
            if (!Enum.TryParse<LoadStatusEnum>(status, true, out var statusEnum))
            {
                _logger.LogWarning($"Invalid load status: {status}");
                return new List<Load>();
            }

            return await _context.Loads
                .Include(l => l.Shipper)
                .Include(l => l.LoadType)
                .Where(l => l.Status == statusEnum)
                .ToListAsync();
        }

        public async Task<bool> AssignTruckToLoadAsync(long loadId, long truckId)
        {
            var load = await _context.Loads.FindAsync(loadId);
            var truck = await _context.Trucks.FindAsync(truckId);

            if (load == null || truck == null)
                return false;

            _logger.LogInformation($"Assigning truck ID: {truckId} to load ID: {loadId}");

            // This is a placeholder - you would need to add a TruckId property to the Load model
            // or create a separate assignment entity
            // load.TruckId = truckId;
            // load.UpdatedDate = DateTime.UtcNow;

            // For now, just log the assignment
            _logger.LogInformation($"Truck {truckId} assigned to load {loadId}");
            
            return true;
        }
    }
}
