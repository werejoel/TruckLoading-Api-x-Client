using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(UserManager<User> userManager, ILogger<UserProfileService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<User?> GetUserProfile(string userId)
        {
            return await _userManager.Users
                .Include(u => u.ContactDetails)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateUserProfile(string userId, UserProfileUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                return false;
            }

            user.PhoneNumber = request.PhoneNumber;
            user.CompanyName = request.CompanyName;
            
            // Only update TruckOwnerType if it's not null
            if (request.TruckOwnerType.HasValue)
            {
                user.TruckOwnerType = request.TruckOwnerType.Value;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
