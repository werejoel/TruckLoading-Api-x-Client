using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class TruckTypeService : ITruckTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckTypeService> _logger;

        public TruckTypeService(ApplicationDbContext context, ILogger<TruckTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TruckType>> GetAllTruckTypesAsync()
        {
            return await _context.TruckTypes
                .Where(tt => tt.IsActive)
                .OrderBy(tt => tt.Name)
                .ToListAsync();
        }

        public async Task<TruckType?> GetTruckTypeByIdAsync(int id)
        {
            return await _context.TruckTypes
                .FirstOrDefaultAsync(tt => tt.Id == id && tt.IsActive);
        }

        public async Task<TruckType> CreateTruckTypeAsync(TruckType truckType)
        {
            truckType.CreatedDate = DateTime.UtcNow;
            truckType.IsActive = true;
            
            _context.TruckTypes.Add(truckType);
            await _context.SaveChangesAsync();
            
            return truckType;
        }

        public async Task<bool> UpdateTruckTypeAsync(TruckType truckType)
        {
            var existingTruckType = await _context.TruckTypes.FindAsync(truckType.Id);
            if (existingTruckType == null || !existingTruckType.IsActive) return false;
            
            existingTruckType.Name = truckType.Name;
            existingTruckType.UpdatedDate = DateTime.UtcNow;
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTruckTypeAsync(int id)
        {
            var truckType = await _context.TruckTypes.FindAsync(id);
            if (truckType == null) return false;
            
            // Soft delete
            truckType.IsActive = false;
            truckType.UpdatedDate = DateTime.UtcNow;
            
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 