using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class LoadTypeService : ILoadTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoadTypeService> _logger;

        public LoadTypeService(
            ApplicationDbContext context,
            ILogger<LoadTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<LoadType>> GetAllLoadTypesAsync()
        {
            try
            {
                return await _context.LoadTypes
                    .Where(lt => lt.IsActive)
                    .OrderBy(lt => lt.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all load types");
                throw;
            }
        }

        public async Task<LoadType?> GetLoadTypeByIdAsync(int id)
        {
            try
            {
                return await _context.LoadTypes.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving load type with ID {Id}", id);
                throw;
            }
        }

        public async Task<LoadType> CreateLoadTypeAsync(LoadType loadType)
        {
            try
            {
                _context.LoadTypes.Add(loadType);
                await _context.SaveChangesAsync();
                return loadType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating load type");
                throw;
            }
        }

        public async Task<bool> UpdateLoadTypeAsync(LoadType loadType)
        {
            try
            {
                _context.Entry(loadType).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating load type with ID {Id}", loadType.Id);
                throw;
            }
        }

        public async Task<bool> DeleteLoadTypeAsync(int id)
        {
            try
            {
                var loadType = await _context.LoadTypes.FindAsync(id);
                if (loadType == null)
                {
                    return false;
                }

                loadType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting load type with ID {Id}", id);
                throw;
            }
        }
    }
} 