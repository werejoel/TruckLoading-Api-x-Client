using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Domain.Enums;
using Asp.Versioning;

namespace TruckLoadingApp.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Shipper")] // Ensure only Shippers can access this controller
    public class LoadController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILoadService _loadService;
        private readonly ILogger<LoadController> _logger;

        public LoadController(UserManager<User> userManager, ILoadService loadService, ILogger<LoadController> logger)
        {
            _userManager = userManager;
            _loadService = loadService;
            _logger = logger; // Inject logger
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostLoad([FromBody] LoadPostRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state."); // Log invalid model state
                return BadRequest(ModelState);
            }

            // Get user ID from the NameIdentifier claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims.");
                return Unauthorized("User not found");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return Unauthorized("User not found");
                }

                // Create the Load object
                var load = new Load
                {
                    ShipperId = userId,
                    Weight = request.Weight,
                    Description = request.Description,
                    Height = request.Height,
                    Width = request.Width,
                    Length = request.Length,
                    SpecialRequirements = request.SpecialRequirements,
                    PickupDate = request.PickupDate,
                    DeliveryDate = request.DeliveryDate,
                    GoodsType = request.GoodsType,
                    Status = LoadStatusEnum.Pending,
                    RequiredTruckTypeId = request.RequiredTruckTypeId
                };

                // Use the LoadService to create the load
                var result = await _loadService.CreateLoad(load);

                if (result)
                {
                    _logger.LogInformation($"Load created successfully for User ID: {userId}");
                    return Ok("Load posted successfully");
                }
                else
                {
                    _logger.LogError($"Failed to create load for User ID: {userId}");
                    return BadRequest("Failed to post load");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error posting load for User ID: {userId}");
                return StatusCode(500, "Internal server error"); // Return a more generic error message
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoadById(long id)
        {
            try
            {
                var load = await _loadService.GetLoadById(id);

                if (load != null)
                {
                    _logger.LogInformation($"Load retrieved successfully with ID: {id}");
                    return Ok(load);
                }
                else
                {
                    _logger.LogWarning($"Load with ID {id} not found.");
                    return NotFound("Load not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting load by ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")] // Only Admins can access this action
        public async Task<IActionResult> GetAllLoads()
        {
            try
            {
                var loads = await _loadService.GetAllLoads();
                _logger.LogInformation("All loads retrieved successfully.");
                return Ok(loads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all loads.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelLoad(long id)
        {
            try
            {
                var success = await _loadService.CancelLoad(id);

                if (success)
                {
                    _logger.LogInformation($"Load with ID {id} cancelled successfully.");
                    return Ok("Load cancelled");
                }
                else
                {
                    _logger.LogWarning($"Could not cancel load with ID {id}.");
                    return BadRequest("Could not cancel load");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling load with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}