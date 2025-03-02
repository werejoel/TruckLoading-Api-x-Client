using TruckLoadingApp.Domain.DTOs;

namespace TruckLoadingApp.Application.Services.Authentication.Interfaces
{
    public interface IAuthService
    {
        // Legacy registration method for backward compatibility
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        
        // Role-specific registration methods
        Task<AuthResponseDto?> RegisterShipperAsync(ShipperRegisterDto registerDto);
        Task<AuthResponseDto?> RegisterTruckerAsync(TruckerRegisterDto registerDto);
        Task<AuthResponseDto?> RegisterCompanyAsync(CompanyRegisterDto registerDto);
        Task<AuthResponseDto?> RegisterAdminAsync(AdminRegisterDto registerDto);
        
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(LogoutDto logoutDto);
    }
} 