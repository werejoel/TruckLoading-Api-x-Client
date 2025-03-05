using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TruckLoadingApp.Application.Services.Authentication.Interfaces;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Domain.DTOs;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDriverService _driverService;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            ApplicationDbContext context,
            IDriverService driverService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _driverService = driverService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var userExists = await _userManager.FindByEmailAsync(registerDto.Username);
                if (userExists != null)
                {
                    _logger.LogWarning($"Registration failed: User {registerDto.Username} already exists");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User already exists!"
                    };
                }

                // Create new user
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = registerDto.UserType,
                    TruckOwnerType = registerDto.TruckOwnerType,
                    CompanyName = registerDto.CompanyName,
                    CompanyAddress = registerDto.CompanyAddress,
                    CompanyRegistrationNumber = registerDto.CompanyRegistrationNumber,
                    CompanyContact = registerDto.CompanyContact,
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Registration failed: {errors}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"User creation failed: {errors}"
                    };
                }

                // Assign role based on user type
                string roleName = registerDto.UserType.ToString();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                // Generate tokens
                var token = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully!",
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return null;
            }
        }

        // Shipper registration
        public async Task<AuthResponseDto?> RegisterShipperAsync(ShipperRegisterDto registerDto)
        {
            try
            {
                // Create user with shipper-specific properties
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = UserType.Shipper,
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                return await RegisterUserAsync(user, registerDto.Password, UserType.Shipper.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during shipper registration");
                return null;
            }
        }

        // Trucker registration
        public async Task<AuthResponseDto?> RegisterTruckerAsync(TruckerRegisterDto registerDto)
        {
            try
            {
                // Create user with trucker-specific properties
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = UserType.Trucker,
                    TruckOwnerType = registerDto.TruckOwnerType,
                    PhoneNumber = registerDto.PhoneNumber,
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                // Register the user account
                var authResponse = await RegisterUserAsync(user, registerDto.Password, UserType.Trucker.ToString());
                
                if (authResponse != null && authResponse.Success)
                {
                    try
                    {
                        // Create driver record with license details
                        var driver = new Driver
                        {
                            UserId = authResponse.UserId,
                            FirstName = registerDto.FirstName, 
                            LastName = registerDto.LastName,
                            LicenseNumber = registerDto.LicenseNumber,
                            LicenseExpiryDate = registerDto.LicenseExpiryDate,
                            Experience = registerDto.Experience,
                            PhoneNumber = registerDto.PhoneNumber,
                            IsAvailable = true,
                            CreatedDate = DateTime.UtcNow
                        };
                        
                        await _driverService.CreateDriverAsync(driver);
                        _logger.LogInformation("Driver record created for trucker {Email}", registerDto.Username);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating driver record for trucker {Email}", registerDto.Username);
                        // Continue anyway, as the user account was created successfully
                    }
                }

                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during trucker registration");
                return null;
            }
        }

        // Company registration
        public async Task<AuthResponseDto?> RegisterCompanyAsync(CompanyRegisterDto registerDto)
        {
            try
            {
                // Create user with company-specific properties
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = UserType.Company,
                    CompanyName = registerDto.CompanyName,
                    CompanyAddress = registerDto.CompanyAddress,
                    CompanyRegistrationNumber = registerDto.CompanyRegistrationNumber,
                    CompanyContact = registerDto.CompanyContact,
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                return await RegisterUserAsync(user, registerDto.Password, UserType.Company.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during company registration");
                return null;
            }
        }

        // Admin registration
        public async Task<AuthResponseDto?> RegisterAdminAsync(AdminRegisterDto registerDto)
        {
            try
            {
                // Create user with admin-specific properties
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = UserType.Admin,
                    CreatedDate = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                return await RegisterUserAsync(user, registerDto.Password, UserType.Admin.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin registration");
                return null;
            }
        }

        // Common registration logic
        private async Task<AuthResponseDto?> RegisterUserAsync(User user, string password, string roleName)
        {
            // Check if user already exists
            var userExists = await _userManager.FindByEmailAsync(user.Email);
            if (userExists != null)
            {
                _logger.LogWarning($"Registration failed: User {user.Email} already exists");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists!"
                };
            }

            // Create user
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Registration failed: {errors}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"User creation failed: {errors}"
                };
            }

            // Assign role
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

            // Generate tokens
            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully!",
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                UserId = user.Id,
                Username = user.UserName ?? string.Empty,
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(loginDto.Username);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User {loginDto.Username} not found");
                    return null;
                }

                // Check password
                if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {loginDto.Username}");
                    return null;
                }

                // Generate tokens
                var token = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful!",
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return null;
            }
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                // Validate JWT token
                var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
                if (principal == null)
                {
                    _logger.LogWarning("Invalid token during refresh");
                    return null;
                }

                // Extract user ID from claims
                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token claims");
                    return null;
                }

                // Find user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning($"Refresh token invalid or expired for user ID {userId}");
                    return null;
                }

                // Generate new tokens
                var newToken = await GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Save new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token refreshed successfully!",
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return null;
            }
        }

        public async Task<bool> LogoutAsync(LogoutDto logoutDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(logoutDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning($"Logout failed: User with ID {logoutDto.UserId} not found");
                    return false;
                }

                // Clear refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        #region Helper Methods

        private async Task<string> GenerateJwtToken(User user)
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false, // Don't validate lifetime here
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        #endregion
    }
}