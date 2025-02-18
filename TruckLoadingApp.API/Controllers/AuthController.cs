using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TruckLoadingApp.API.Models.Requests;
using TruckLoadingApp.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Controller for handling authentication-related operations such as registration and login.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The UserManager for managing users.</param>
    /// <param name="roleManager">The RoleManager for managing roles.</param>
    /// <param name="configuration">The IConfiguration for accessing configuration settings.</param>
    /// <param name="logger">The ILogger for logging.</param>
    public AuthController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">The registration request containing user details.</param>
    /// <returns>An IActionResult indicating the result of the registration.</returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Invalid model or user already exists.</response>
    /// <response code="500">Failed to assign user role.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state during registration");
            return BadRequest(ModelState);
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            _logger.LogWarning($"Registration attempt with existing email: {model.Email}");
            return BadRequest(new { Message = "User with this email already exists." });
        }

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            UserType = model.UserType,
            CompanyName = model.CompanyName,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Convert UserType enum to role name
            string roleName = user.UserType.ToString();

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation($"Created new role: {roleName}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (roleResult.Succeeded)
            {
                _logger.LogInformation($"User {user.Email} registered successfully with role: {roleName}");

                // Generate token immediately after registration
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);

                return Ok(new
                {
                    Message = "Registration successful",
                    Token = token,
                    User = new
                    {
                        user.Email,
                        user.UserType,
                        user.CompanyName,
                        Roles = roles
                    }
                });
            }
            else
            {
                _logger.LogError($"Failed to assign role {roleName} to user {user.Email}");
                // Clean up the created user since role assignment failed
                await _userManager.DeleteAsync(user);
                return StatusCode(500, new { Message = "Failed to assign user role", Errors = roleResult.Errors });
            }
        }

        _logger.LogError($"User creation failed for {model.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="model">The login request containing user credentials.</param>
    /// <returns>An IActionResult containing the JWT token upon successful login.</returns>
    /// <response code="200">Returns JWT token and user information on successful login.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1), // Token expiration
            User = new
            {
                user.Email,
                user.UserType,
                user.CompanyName,
                Roles = roles
            }
        });
    }

    /// <summary>
    /// Generates a JWT token for the given user and roles.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="roles">The roles of the user.</param>
    /// <returns>A JWT token string.</returns>
    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim("UserType", user.UserType.ToString())
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
        }
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
