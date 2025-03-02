using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Administration.Interfaces;
using TruckLoadingApp.Domain.DTOs;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.Administration
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(userUpdateDto);
                if (!result)
                {
                    return BadRequest("User update failed");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {Id}", userUpdateDto.UserId);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return BadRequest("User deletion failed");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDto changeRoleDto)
        {
            try
            {
                var result = await _userService.ChangeUserRoleAsync(changeRoleDto);
                if (!result)
                {
                    return BadRequest("Role change failed");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role for user with ID {Id}", changeRoleDto.UserId);
                return StatusCode(500, "An error occurred while changing the user's role");
            }
        }
    }
} 