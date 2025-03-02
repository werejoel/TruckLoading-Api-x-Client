using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Domain.DTOs;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Application.Services.Authentication.Interfaces;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Infrastructure.Data;
using TruckLoadingApp.API.Models.Requests;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.API.Controllers
{
   // [EnableRateLimiting("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Company")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CompanyController> _logger;
        private readonly IAuthService _authService;
        private readonly IDriverService _driverService;
        private readonly IConfiguration _configuration;

        public CompanyController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<CompanyController> logger,
            IAuthService authService,
            IDriverService driverService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _authService = authService;
            _driverService = driverService;
            _configuration = configuration;
        }

        // Get all truckers associated with the company
        [HttpGet("truckers")]
        public async Task<ActionResult<IEnumerable<TruckerDto>>> GetTruckers()
        {
            try
            {
                var companyName = User.FindFirstValue("CompanyName");
                if (string.IsNullOrEmpty(companyName))
                {
                    _logger.LogWarning("CompanyName claim not found in token");
                    return Unauthorized(new { Message = "Company name not found in token. Please refresh your token." });
                }

                _logger.LogInformation("Fetching truckers for company {CompanyName}", companyName);

                try
                {
                    // Get all users with Trucker role associated with this company
                    var truckerUsers = await _userManager.GetUsersInRoleAsync("Trucker");
                    var companyTruckers = truckerUsers
                        .Where(t => t.CompanyName == companyName)
                        .Select(t => new
                        {
                            t.Id,
                            t.FirstName,
                            t.LastName,
                            t.PhoneNumber,
                            t.Email,
                            TruckOwnerType = t.TruckOwnerType.HasValue ? t.TruckOwnerType.Value.ToString() : "Unknown"
                        })
                        .ToList();

                    var truckerDtos = companyTruckers.Select(t => new TruckerDto
                    {
                        Id = t.Id,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Email = t.Email,
                        PhoneNumber = t.PhoneNumber ?? string.Empty,
                        TruckOwnerType = t.TruckOwnerType,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();

                    _logger.LogInformation("Found {Count} truckers for company {CompanyName}", truckerDtos.Count, companyName);
                    return Ok(truckerDtos);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError("Database error while retrieving company truckers: {Error}", dbEx.Message);
                    
                    // Fallback to an empty list if there's a database schema issue
                    _logger.LogWarning("Returning empty trucker list due to database error");
                    return Ok(new List<TruckerDto>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving company truckers: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while retrieving truckers.", Error = ex.Message });
            }
        }

        // Register a new trucker for the company
        [HttpPost("register-trucker")]
        public async Task<IActionResult> RegisterTrucker([FromBody] TruckerRegisterRequest model)
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var companyName = User.FindFirstValue("CompanyName");
                if (string.IsNullOrEmpty(companyName))
                {
                    _logger.LogWarning("CompanyName claim not found in token");
                    return Unauthorized(new { Message = "Company name not found in token. Please refresh your token." });
                }

                _logger.LogInformation("Attempting to register a new trucker for company {CompanyName}", companyName);

                // Create the user
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TruckOwnerType = model.TruckOwnerType,
                    UserType = UserType.Trucker,
                    CompanyName = companyName,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create trucker account: {Errors}", errors);
                    return BadRequest(new { Message = "Failed to create account", Errors = errors });
                }

                // Assign the Trucker role
                await _userManager.AddToRoleAsync(user, "Trucker");
                _logger.LogInformation("Successfully registered trucker {Email} for company {CompanyName}", model.Email, companyName);

                return Ok(new { Message = "Trucker registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering trucker: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while registering the trucker.", Error = ex.Message });
            }
        }

        // Get all trucks associated with the company
        [HttpGet("trucks")]
        public async Task<ActionResult<IEnumerable<TruckDto>>> GetTrucks()
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching trucks for company with ID {CompanyId}", companyId);

                try
                {
                    // Get all trucks owned by this company
                    // Use a more specific query to avoid schema mismatches
                    var trucks = await _context.Trucks
                        .Where(t => t.OwnerId == companyId)
                        .Select(t => new
                        {
                            t.Id,
                            t.NumberPlate,
                            t.TruckTypeId,
                            t.LoadCapacityWeight,
                            t.IsApproved,
                            t.OperationalStatus
                        })
                        .ToListAsync();

                    // Get truck types separately to avoid join issues
                    var truckTypeIds = trucks.Select(t => t.TruckTypeId).Distinct().ToList();
                    var truckTypes = await _context.TruckTypes
                        .Where(tt => truckTypeIds.Contains(tt.Id))
                        .ToDictionaryAsync(tt => tt.Id, tt => tt.Name);

                    var truckDtos = trucks.Select(t => new TruckDto
                    {
                        Id = t.Id,
                        NumberPlate = t.NumberPlate,
                        TruckType = truckTypes.TryGetValue(t.TruckTypeId, out var typeName) ? typeName : "Unknown",
                        LoadCapacityWeight = t.LoadCapacityWeight,
                        IsApproved = t.IsApproved,
                        OperationalStatus = t.OperationalStatus.ToString()
                    }).ToList();

                    _logger.LogInformation("Found {Count} trucks for company with ID {CompanyId}", truckDtos.Count, companyId);
                    return Ok(truckDtos);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error while retrieving company trucks");
                    
                    // Fallback to an empty list if there's a database schema issue
                    _logger.LogWarning("Returning empty truck list due to database error");
                    return Ok(new List<TruckDto>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company trucks");
                return StatusCode(500, new { Message = "An error occurred while retrieving trucks.", Error = ex.Message });
            }
        }

        // Add a new endpoint to refresh the company token
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshCompanyToken()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Refreshing token for user with ID {UserId}", userId);

                // Get the user from the database
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return NotFound(new { Message = "User not found" });
                }

                // Check if the user has company information
                if (string.IsNullOrEmpty(user.CompanyName))
                {
                    _logger.LogWarning("User with ID {UserId} does not have company information", userId);
                    return BadRequest(new { Message = "User does not have company information" });
                }

                // Generate a new token with company information
                var token = await GenerateJwtTokenAsync(user);
                
                _logger.LogInformation("Successfully refreshed token for user with ID {UserId}", userId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error refreshing company token: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while refreshing the token.", Error = ex.Message });
            }
        }

        // Get all drivers associated with the company
        [HttpGet("drivers")]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetDrivers()
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching drivers for company with ID {CompanyId}", companyId);

                var drivers = await _driverService.GetDriversByCompanyAsync(companyId);
                var driverDtos = drivers.Select(DriverDto.FromDriver).ToList();

                return Ok(driverDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company drivers");
                return StatusCode(500, new { Message = "An error occurred while retrieving drivers.", Error = ex.Message });
            }
        }

        // Register a new driver for the company
        [HttpPost("register-driver")]
        public async Task<IActionResult> RegisterDriver([FromBody] DriverRegisterRequest model)
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var companyName = User.FindFirstValue("CompanyName");
                if (string.IsNullOrEmpty(companyName))
                {
                    _logger.LogWarning("CompanyName claim not found in token");
                    return Unauthorized(new { Message = "Company name not found in token. Please refresh your token." });
                }

                _logger.LogInformation("Attempting to register a new driver for company {CompanyName}", companyName);

                // Check if user with this email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                string userId;

                if (existingUser == null)
                {
                    // Create a new user for the driver
                    var user = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        UserType = UserType.Trucker, // Using Trucker type for drivers
                        CompanyName = companyName,
                        CreatedDate = DateTime.UtcNow
                    };

                    // Generate a random password (or use a default one that must be changed)
                    var password = Guid.NewGuid().ToString("N").Substring(0, 8) + "A1!";
                    var result = await _userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogWarning("Failed to create user for driver: {Errors}", errors);
                        return BadRequest(new { Message = "Failed to create user account", Errors = errors });
                    }

                    // Add to Trucker role (since "Driver" role doesn't exist)
                    await _userManager.AddToRoleAsync(user, "Trucker");
                    userId = user.Id;

                    // TODO: Send email with credentials to the driver
                }
                else
                {
                    // Use existing user
                    userId = existingUser.Id;
                }

                // Create the driver record
                var driver = new Driver
                {
                    UserId = userId,
                    CompanyId = companyId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    LicenseNumber = model.LicenseNumber,
                    LicenseExpiryDate = model.LicenseExpiryDate,
                    Experience = model.Experience,
                    IsAvailable = true
                };

                var createdDriver = await _driverService.RegisterDriverForCompanyAsync(driver, companyId);

                return Ok(new { 
                    Message = "Driver registered successfully", 
                    DriverId = createdDriver.Id 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering driver: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while registering the driver.", Error = ex.Message });
            }
        }

        // Assign a driver to a truck
        [HttpPost("assign-driver")]
        public async Task<IActionResult> AssignDriverToTruck([FromBody] AssignDriverRequest model)
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Verify the driver belongs to this company
                var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == model.DriverId);
                if (driver == null)
                {
                    return NotFound(new { Message = "Driver not found" });
                }

                if (driver.CompanyId != companyId)
                {
                    return Forbid();
                }

                // Verify the truck belongs to this company
                var truck = await _context.Trucks.FirstOrDefaultAsync(t => t.Id == model.TruckId);
                if (truck == null)
                {
                    return NotFound(new { Message = "Truck not found" });
                }

                if (truck.OwnerId != companyId)
                {
                    return Forbid();
                }

                // Assign the driver to the truck
                var result = await _driverService.AssignDriverToTruckAsync(model.DriverId, model.TruckId);
                if (!result)
                {
                    return BadRequest(new { Message = "Failed to assign driver to truck" });
                }

                return Ok(new { Message = "Driver assigned to truck successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning driver to truck");
                return StatusCode(500, new { Message = "An error occurred while assigning the driver to the truck.", Error = ex.Message });
            }
        }

        // Register a new truck for the company
        [HttpPost("register-truck")]
        public async Task<IActionResult> RegisterTruck([FromBody] TruckRegistrationRequest model)
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Attempting to register a new truck for company with ID {CompanyId}", companyId);

                // Create the truck
                var truck = new Truck
                {
                    OwnerId = companyId,
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

                _context.Trucks.Add(truck);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully registered truck with ID {TruckId} for company with ID {CompanyId}", truck.Id, companyId);

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

        // Get truck by ID
        [HttpGet("trucks/{id}")]
        public async Task<ActionResult<TruckDetailDto>> GetTruckById(int id)
        {
            try
            {
                var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(companyId))
                {
                    _logger.LogWarning("CompanyId claim not found in token");
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                _logger.LogInformation("Fetching truck with ID {TruckId} for company with ID {CompanyId}", id, companyId);

                // Get the truck
                var truck = await _context.Trucks
                    .Where(t => t.Id == id && t.OwnerId == companyId)
                    .FirstOrDefaultAsync();

                if (truck == null)
                {
                    _logger.LogWarning("Truck with ID {TruckId} not found for company with ID {CompanyId}", id, companyId);
                    return NotFound(new { Message = "Truck not found" });
                }

                // Get the truck type
                var truckType = await _context.TruckTypes
                    .Where(tt => tt.Id == truck.TruckTypeId)
                    .FirstOrDefaultAsync();

                // Get the assigned driver (if any)
                string? assignedDriverName = null;
                if (truck.AssignedDriverId.HasValue)
                {
                    var driver = await _context.Drivers
                        .Where(d => d.Id == truck.AssignedDriverId.Value)
                        .FirstOrDefaultAsync();
                    
                    if (driver != null)
                    {
                        assignedDriverName = $"{driver.FirstName} {driver.LastName}";
                    }
                }

                var truckDto = new TruckDetailDto
                {
                    Id = truck.Id,
                    NumberPlate = truck.NumberPlate,
                    TruckType = truckType?.Name ?? "Unknown",
                    LoadCapacityWeight = truck.LoadCapacityWeight,
                    LoadCapacityVolume = truck.LoadCapacityVolume,
                    Height = truck.Height,
                    Width = truck.Width,
                    Length = truck.Length,
                    AvailabilityStartDate = truck.AvailabilityStartDate,
                    AvailabilityEndDate = truck.AvailabilityEndDate,
                    IsApproved = truck.IsApproved,
                    OperationalStatus = truck.OperationalStatus.ToString(),
                    AssignedDriverId = truck.AssignedDriverId,
                    AssignedDriverName = assignedDriverName,
                    CreatedDate = truck.CreatedDate,
                    UpdatedDate = truck.UpdatedDate
                };

                return Ok(truckDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck with ID {TruckId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving the truck.", Error = ex.Message });
            }
        }

        // Helper method to generate JWT token
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add company-specific claims if the user is a company
            if (userRoles.Contains("Company"))
            {
                if (!string.IsNullOrEmpty(user.CompanyName))
                {
                    authClaims.Add(new Claim("CompanyName", user.CompanyName));
                }
                
                if (!string.IsNullOrEmpty(user.CompanyRegistrationNumber))
                {
                    authClaims.Add(new Claim("CompanyRegistrationNumber", user.CompanyRegistrationNumber));
                }
            }

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs
    public class TruckerDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string TruckOwnerType { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }

    public class TruckDto
    {
        public int Id { get; set; }
        public string NumberPlate { get; set; } = string.Empty;
        public string TruckType { get; set; } = string.Empty;
        public decimal LoadCapacityWeight { get; set; }
        public bool IsApproved { get; set; }
        public string OperationalStatus { get; set; } = string.Empty;
    }

    public class CompanyTruckerRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class AssignDriverRequest
    {
        [Required]
        public long DriverId { get; set; }

        [Required]
        public int TruckId { get; set; }
    }

    public class TruckDetailDto
    {
        public int Id { get; set; }
        public string NumberPlate { get; set; } = string.Empty;
        public string TruckType { get; set; } = string.Empty;
        public decimal LoadCapacityWeight { get; set; }
        public decimal LoadCapacityVolume { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public DateTime AvailabilityStartDate { get; set; }
        public DateTime AvailabilityEndDate { get; set; }
        public bool IsApproved { get; set; }
        public string OperationalStatus { get; set; } = string.Empty;
        public long? AssignedDriverId { get; set; }
        public string? AssignedDriverName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
} 