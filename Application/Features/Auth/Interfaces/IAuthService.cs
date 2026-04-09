using StayHub.Backend.Application.Features.Auth.DTOs;

namespace StayHub.Backend.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> LogoutAsync(string userId);
}
