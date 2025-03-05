using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Authentication.Interfaces;
using TruckLoadingApp.Domain.DTOs;
using System.Linq;

namespace TruckLoadingApp.API.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var result = await _authService.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized(new {
                        Success = false,
                        Message = "Invalid username or password"
                    });
                }

                if (!result.Success)
                {
                    return Unauthorized(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", loginDto.Username);
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred during login",
                    Error = ex.Message
                });
            }
        }

        // Legacy registration endpoint for backward compatibility
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerDto);
                if (result == null)
                {
                    return BadRequest("Registration failed");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}", registerDto.Username);
                return StatusCode(500, "An error occurred during registration");
            }
        }

        // Shipper-specific registration endpoint
        [HttpPost("register/shipper")]
        public async Task<ActionResult<AuthResponseDto>> RegisterShipper([FromBody] ShipperRegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var result = await _authService.RegisterShipperAsync(registerDto);
                if (result == null)
                {
                    return BadRequest(new { 
                        Success = false,
                        Message = "Shipper registration failed. Please check your input and try again."
                    });
                }

                if (!result.Success)
                {
                    return BadRequest(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during shipper registration for user {Username}", registerDto.Username);
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred during shipper registration",
                    Error = ex.Message
                });
            }
        }

        // Trucker-specific registration endpoint
        [HttpPost("register/trucker")]
        public async Task<ActionResult<AuthResponseDto>> RegisterTrucker([FromBody] TruckerRegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    _logger.LogWarning("Validation failed for trucker registration: {Errors}", string.Join(", ", errors));
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                _logger.LogInformation("Attempting to register trucker with username {Username}", registerDto.Username);
                
                var result = await _authService.RegisterTruckerAsync(registerDto);
                if (result == null)
                {
                    return BadRequest(new { 
                        Success = false,
                        Message = "Trucker registration failed. Please check your input and try again."
                    });
                }

                if (!result.Success)
                {
                    return BadRequest(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during trucker registration for user {Username}", registerDto.Username);
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred during trucker registration",
                    Error = ex.Message
                });
            }
        }

        // Company-specific registration endpoint
        [HttpPost("register/company")]
        public async Task<ActionResult<AuthResponseDto>> RegisterCompany([FromBody] CompanyRegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var result = await _authService.RegisterCompanyAsync(registerDto);
                if (result == null)
                {
                    return BadRequest(new { 
                        Success = false,
                        Message = "Company registration failed. Please check your input and try again."
                    });
                }

                if (!result.Success)
                {
                    return BadRequest(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during company registration for user {Username}", registerDto.Username);
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred during company registration",
                    Error = ex.Message
                });
            }
        }

        // Admin-specific registration endpoint
        [HttpPost("register/admin")]
        public async Task<ActionResult<AuthResponseDto>> RegisterAdmin([FromBody] AdminRegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var result = await _authService.RegisterAdminAsync(registerDto);
                if (result == null)
                {
                    return BadRequest(new { 
                        Success = false,
                        Message = "Admin registration failed. Please check your input and try again."
                    });
                }

                if (!result.Success)
                {
                    return BadRequest(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin registration for user {Username}", registerDto.Username);
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred during admin registration",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new { 
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var result = await _authService.RefreshTokenAsync(refreshTokenDto);
                if (result == null)
                {
                    return Unauthorized(new {
                        Success = false,
                        Message = "Invalid refresh token"
                    });
                }

                if (!result.Success)
                {
                    return Unauthorized(result); // Return the error response from the service
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new {
                    Success = false,
                    Message = "An error occurred while refreshing the token",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LogoutAsync(logoutDto);
                if (!result)
                {
                    return BadRequest("Logout failed");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", logoutDto.UserId);
                return StatusCode(500, "An error occurred during logout");
            }
        }
    }
} 