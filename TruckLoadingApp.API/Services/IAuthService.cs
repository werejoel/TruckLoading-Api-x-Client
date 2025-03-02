using System.Threading.Tasks;
using TruckLoadingApp.API.DTOs;
using TruckLoadingApp.API.Models;

namespace TruckLoadingApp.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterTruckerAsync(TruckerRegisterDto registerDto);
        Task<AuthResponseDto> RegisterCompanyAsync(CompanyRegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
} 