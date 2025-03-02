using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Administration.Interfaces;
using TruckLoadingApp.Domain.DTOs;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Administration
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserType = user.UserType,
                        TruckOwnerType = user.TruckOwnerType,
                        CompanyName = user.CompanyName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles.ToList(),
                        CreatedDate = user.CreatedDate
                    });
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return Enumerable.Empty<UserDto>();
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found");
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);
                return new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType,
                    TruckOwnerType = user.TruckOwnerType,
                    CompanyName = user.CompanyName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList(),
                    CreatedDate = user.CreatedDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID {id}");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userUpdateDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning($"Update failed: User with ID {userUpdateDto.UserId} not found");
                    return false;
                }

                // Update user properties if provided
                if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
                    user.FirstName = userUpdateDto.FirstName;

                if (!string.IsNullOrEmpty(userUpdateDto.LastName))
                    user.LastName = userUpdateDto.LastName;

                if (!string.IsNullOrEmpty(userUpdateDto.Email))
                    user.Email = userUpdateDto.Email;

                if (!string.IsNullOrEmpty(userUpdateDto.PhoneNumber))
                    user.PhoneNumber = userUpdateDto.PhoneNumber;

                // Update company-specific fields if provided
                if (!string.IsNullOrEmpty(userUpdateDto.CompanyName))
                    user.CompanyName = userUpdateDto.CompanyName;

                if (!string.IsNullOrEmpty(userUpdateDto.CompanyAddress))
                    user.CompanyAddress = userUpdateDto.CompanyAddress;

                if (!string.IsNullOrEmpty(userUpdateDto.CompanyRegistrationNumber))
                    user.CompanyRegistrationNumber = userUpdateDto.CompanyRegistrationNumber;

                if (!string.IsNullOrEmpty(userUpdateDto.CompanyContact))
                    user.CompanyContact = userUpdateDto.CompanyContact;

                // Update trucker-specific fields if provided
                if (userUpdateDto.TruckOwnerType.HasValue)
                    user.TruckOwnerType = userUpdateDto.TruckOwnerType;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"User update failed: {errors}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with ID {userUpdateDto.UserId}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"Delete failed: User with ID {id} not found");
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"User deletion failed: {errors}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID {id}");
                return false;
            }
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleDto changeRoleDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(changeRoleDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning($"Role change failed: User with ID {changeRoleDto.UserId} not found");
                    return false;
                }

                // Check if the role exists
                if (!await _roleManager.RoleExistsAsync(changeRoleDto.NewRole))
                {
                    _logger.LogWarning($"Role change failed: Role {changeRoleDto.NewRole} does not exist");
                    return false;
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove current roles
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                        _logger.LogError($"Failed to remove current roles: {errors}");
                        return false;
                    }
                }

                // Add new role
                var addResult = await _userManager.AddToRoleAsync(user, changeRoleDto.NewRole);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to add new role: {errors}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing role for user with ID {changeRoleDto.UserId}");
                return false;
            }
        }
    }
} 