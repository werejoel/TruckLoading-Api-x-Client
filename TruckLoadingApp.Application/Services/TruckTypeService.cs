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
                .Include(tt => tt.Category)
                .Where(tt => tt.IsActive)
                .OrderBy(tt => tt.Category.CategoryName)
                .ThenBy(tt => tt.Name)
                .ToListAsync();
        }

        public async Task<TruckType?> GetTruckTypeByIdAsync(int id)
        {
            return await _context.TruckTypes
                .Include(tt => tt.Category)
                .FirstOrDefaultAsync(tt => tt.Id == id && tt.IsActive);
        }

        public async Task<IEnumerable<TruckCategory>> GetAllTruckCategoriesAsync()
        {
            return await _context.TruckCategories
                .Where(tc => tc.IsActive)
                .OrderBy(tc => tc.CategoryName)
                .ToListAsync();
        }

        public async Task<TruckCategory?> GetTruckCategoryByIdAsync(int id)
        {
            return await _context.TruckCategories
                .Include(tc => tc.TruckTypes.Where(tt => tt.IsActive))
                .FirstOrDefaultAsync(tc => tc.Id == id && tc.IsActive);
        }

        public async Task<IEnumerable<TruckType>> GetTruckTypesByCategoryIdAsync(int categoryId)
        {
            return await _context.TruckTypes
                .Include(tt => tt.Category)
                .Where(tt => tt.CategoryId == categoryId && tt.IsActive)
                .OrderBy(tt => tt.Name)
                .ToListAsync();
        }
    }
} 