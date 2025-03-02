using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.API.Configuration;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users.");
                return StatusCode(500, new { Message = "Failed to retrieve users. Please try again later." });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID {id}.");
                return StatusCode(500, new { Message = "Failed to retrieve user. Please try again later." });
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                // Update user properties from the model
                user.CompanyName = model.CompanyName;
                user.CompanyAddress = model.CompanyAddress;
                user.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                user.CompanyContact = model.CompanyContact;

                // Save the changes
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User with ID {id} updated successfully.");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"Failed to update user with ID {id}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return BadRequest(new { Message = "Failed to update user.", Errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with ID {id}.");
                return StatusCode(500, new { Message = "Failed to update user. Please try again later." });
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User with ID {id} deleted successfully.");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"Failed to delete user with ID {id}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return BadRequest(new { Message = "Failed to delete user.", Errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with ID {id}.");
                return StatusCode(500, new { Message = "Failed to delete user. Please try again later." });
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles.");
                return StatusCode(500, new { Message = "Failed to retrieve roles. Please try again later." });
            }
        }

        [HttpPost("roles/create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                if (await _context.Roles.AnyAsync(r => r.Name == model.RoleName))
                {
                    _logger.LogWarning($"Role with name {model.RoleName} already exists.");
                    return BadRequest(new { Message = "Role already exists" });
                }

                var role = new IdentityRole(model.RoleName);
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Role with name {model.RoleName} created successfully.");
                return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);  // Corrected to nameof and include role
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating role with name {model.RoleName}.");
                return StatusCode(500, new { Message = "Failed to create role. Please try again later." });
            }
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role == null)
                {
                    _logger.LogWarning($"Role with ID {id} not found.");
                    return NotFound(new { Message = $"Role with ID {id} not found." });
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Role with ID {id} deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting role with ID {id}.");
                return StatusCode(500, new { Message = "Failed to delete role. Please try again later." });
            }
        }
    }
}
