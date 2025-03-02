using TruckLoadingApp.Domain.DTOs;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Administration.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ChangeUserRoleAsync(ChangeRoleDto changeRoleDto);
    }
} 