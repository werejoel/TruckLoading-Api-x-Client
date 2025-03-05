using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TruckLoadingApp.API.DTOs;
using TruckLoadingApp.API.Models;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"Registration failed: User with email {registerDto.Email} already exists");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    CreatedDate = DateTime.UtcNow
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

                // Assign role
                string roleName = registerDto.UserType.ToString();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                // Generate tokens
                var token = await GenerateJwtTokenAsync(user);
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
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponseDto> RegisterTruckerAsync(TruckerRegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning($"Registration failed: User with email {registerDto.Username} already exists");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    TruckOwnerType = (Enums.TruckOwnerType?)registerDto.TruckOwnerType,
                    PhoneNumber = registerDto.PhoneNumber,
                    CreatedDate = DateTime.UtcNow
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

                // Assign role
                string roleName = "Trucker";
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                // Create driver record for the trucker
                try
                {
                    var driver = new Driver
                    {
                        UserId = user.Id,
                        FirstName = registerDto.FirstName,
                        LastName = registerDto.LastName,
                        LicenseNumber = registerDto.LicenseNumber,
                        LicenseExpiryDate = registerDto.LicenseExpiryDate,
                        Experience = registerDto.Experience,
                        PhoneNumber = registerDto.PhoneNumber,
                        IsAvailable = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Drivers.Add(driver);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created driver record for trucker {Username}", registerDto.Username);
                }
                catch (Exception driverEx)
                {
                    _logger.LogError(driverEx, "Error creating driver record for trucker {Username}", registerDto.Username);
                    // Continue with registration even if driver creation fails
                    // We can handle this separately or provide a way to create the driver record later
                }

                // Generate tokens
                var token = await GenerateJwtTokenAsync(user);
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
                    Message = "Trucker registered successfully!",
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
                _logger.LogError(ex, "Error during trucker registration");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponseDto> RegisterCompanyAsync(CompanyRegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"Registration failed: User with email {registerDto.Email} already exists");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.ContactPersonFirstName,
                    LastName = registerDto.ContactPersonLastName,
                    PhoneNumber = registerDto.PhoneNumber,
                    CompanyName = registerDto.CompanyName,
                    CompanyRegistrationNumber = registerDto.CompanyRegistrationNumber,
                    CreatedDate = DateTime.UtcNow
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

                // Assign role
                string roleName = "Company";
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                // Generate tokens
                var token = await GenerateJwtTokenAsync(user);
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
                    Message = "Company registered successfully!",
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
                _logger.LogError(ex, "Error during company registration");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(loginDto.Username);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User {loginDto.Username} not found");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Check password
                if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {loginDto.Username}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Generate tokens
                var token = await GenerateJwtTokenAsync(user);
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
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                // Validate JWT token
                var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
                if (principal == null)
                {
                    _logger.LogWarning("Invalid token during refresh");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                // Extract user ID from claims
                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token claims");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                // Find user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning($"Refresh token invalid or expired for user ID {userId}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired refresh token"
                    };
                }

                // Generate new tokens
                var newToken = await GenerateJwtTokenAsync(user);
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
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during token refresh"
                };
            }
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
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
    }
} 