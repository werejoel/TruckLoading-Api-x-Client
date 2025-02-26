using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<User?> GetUserProfile(string userId);
        Task<bool> UpdateUserProfile(string userId, UserProfileUpdateRequest request);
        
    }
}
