using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class UserLocationService : IUserLocationService
    {
        private readonly ApplicationDbContext _context;

        public UserLocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserLocation?> GetUserLocation(string userId)
        {
            return await _context.UserLocations
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
