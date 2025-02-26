using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userProfileService, UserManager<User> userManager, ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID is null or empty.");
                    return Unauthorized();
                }

                var user = await _userProfileService.GetUserProfile(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User profile with ID {userId} not found.");
                    return NotFound(new { Message = "User profile not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile.");
                return StatusCode(500, new { Message = "Failed to retrieve user profile. Please try again later." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateRequest model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID is null or empty.");
                    return Unauthorized();
                }

                var success = await _userProfileService.UpdateUserProfile(userId, model);
                if (!success)
                {
                    _logger.LogError($"Failed to update profile for user with ID {userId}.");
                    return BadRequest(new { Message = "Failed to update profile." });
                }

                _logger.LogInformation($"Profile updated successfully for user with ID {userId}.");
                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile.");
                return StatusCode(500, new { Message = "Failed to update user profile. Please try again later." });
            }
        }
    }
}