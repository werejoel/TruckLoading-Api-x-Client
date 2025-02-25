using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;

namespace TruckLoadingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogWarning($"Login failed for email: {model.Email}");
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                return Ok(new
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:DurationInMinutes"])),
                    User = new
                    {
                        user.Email,
                        user.UserType,
                        user.CompanyName,
                        Roles = roles
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for email: {model.Email}");
                return StatusCode(500, new { Message = "Login failed. Please try again later." });
            }
        }

        [HttpPost("register/shipper")]
        public async Task<IActionResult> RegisterShipper([FromBody] ShipperRegisterRequest model)
        {
            return await RegisterUser(model);
        }

        [HttpPost("register/trucker")]
        public async Task<IActionResult> RegisterTrucker([FromBody] TruckerRegisterRequest model)
        {
            return await RegisterUser(model);
        }

        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegisterRequest model)
        {
            return await RegisterUser(model);
        }

        private async Task<IActionResult> RegisterUser(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"User with email {model.Email} already exists.");
                    return BadRequest(new { Message = "User with this email already exists." });
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    UserType = model.UserType,
                    CreatedDate = DateTime.UtcNow
                };

                // Handle Trucker registration
                if (model is TruckerRegisterRequest truckerRequest)
                {
                    user.TruckOwnerType = truckerRequest.TruckOwnerType;
                }

                // Handle Company registration
                if (model is CompanyRegisterRequest companyRequest)
                {
                    user.CompanyName = companyRequest.CompanyName;
                    user.CompanyAddress = companyRequest.CompanyAddress;
                    user.CompanyRegistrationNumber = companyRequest.CompanyRegistrationNumber;
                    user.CompanyContact = companyRequest.CompanyContact;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Registration failed for email {model.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return BadRequest(new { Message = "Registration failed.", Errors = result.Errors });
                }

                await _userManager.AddToRoleAsync(user, model.UserType.ToString());

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                _logger.LogInformation($"User with email {model.Email} registered successfully.");
                return Ok(new { Message = "Registration successful", Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for email: {model.Email}");
                return StatusCode(500, new { Message = "Registration failed. Please try again later." });
            }
        }

        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("UserType", ((int)user.UserType).ToString())
    };

            // Add role claims with the correct claim type to match RoleClaimType in Program.cs
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); ; // This maps to "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}