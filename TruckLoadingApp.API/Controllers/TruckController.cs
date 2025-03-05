using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.API.Models.DTOs;
using System.Security.Claims;

namespace TruckLoadingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Company,Trucker")]
    public class TruckController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TruckController> _logger;

        public TruckController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<TruckController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Register a new truck
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterTruck([FromBody] TruckRegistrationRequest model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Attempting to register a new truck for user with ID {UserId}", userId);

                // Create the truck
                var truck = new Truck
                {
                    OwnerId = userId,
                    TruckTypeId = model.TruckTypeId,
                    NumberPlate = model.NumberPlate,
                    LoadCapacityWeight = model.LoadCapacityWeight,
                    LoadCapacityVolume = model.LoadCapacityVolume,
                    Height = model.Height,
                    Width = model.Width,
                    Length = model.Length,
                    AvailabilityStartDate = model.AvailabilityStartDate,
                    AvailabilityEndDate = model.AvailabilityEndDate,
                    OperationalStatus = TruckOperationalStatusEnum.AwaitingApproval,
                    AvailableCapacityWeight = model.LoadCapacityWeight, // Initially, available capacity equals total capacity
                    CreatedDate = DateTime.UtcNow
                };

                // For truckers, check if they already have a truck (they can only have one)
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && await _userManager.IsInRoleAsync(user, "Trucker"))
                {
                    var existingTruck = await _context.Trucks.FirstOrDefaultAsync(t => t.OwnerId == userId);
                    if (existingTruck != null)
                    {
                        return BadRequest(new { Message = "Individual truckers can only register one truck" });
                    }
                }

                _context.Trucks.Add(truck);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully registered truck with ID {TruckId} for user with ID {UserId}", truck.Id, userId);

                return Ok(new { 
                    Message = "Truck registered successfully", 
                    TruckId = truck.Id 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering truck: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while registering the truck.", Error = ex.Message });
            }
        }

        // Get trucks for the current user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckDto>>> GetTrucks()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching trucks for user with ID {UserId}", userId);

                var trucks = await _context.Trucks
                    .Include(t => t.TruckType)
                    .Where(t => t.OwnerId == userId)
                    .Select(t => new TruckDto
                    {
                        Id = t.Id,
                        NumberPlate = t.NumberPlate,
                        TruckType = t.TruckType != null ? t.TruckType.Name : "Unknown",
                        LoadCapacityWeight = t.LoadCapacityWeight,
                        IsApproved = t.IsApproved,
                        OperationalStatus = t.OperationalStatus.ToString()
                    })
                    .ToListAsync();

                return Ok(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving trucks: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving trucks.", Error = ex.Message });
            }
        }

        // Get truck by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TruckDto>> GetTruckById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var truck = await _context.Trucks
                    .Include(t => t.TruckType)
                    .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

                if (truck == null)
                {
                    return NotFound(new { Message = "Truck not found" });
                }

                var truckDto = new TruckDto
                {
                    Id = truck.Id,
                    NumberPlate = truck.NumberPlate,
                    TruckType = truck.TruckType != null ? truck.TruckType.Name : "Unknown",
                    LoadCapacityWeight = truck.LoadCapacityWeight,
                    IsApproved = truck.IsApproved,
                    OperationalStatus = truck.OperationalStatus.ToString()
                };

                return Ok(truckDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving truck: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving the truck.", Error = ex.Message });
            }
        }
    }
} 