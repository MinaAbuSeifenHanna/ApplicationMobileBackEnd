using StayHub.Backend.Application.Features.Auth.DTOs;

namespace StayHub.Backend.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<ApiResponse<object>> OnboardProfileAsync(string userId, OnboardingProfileRequest profileRequest);
    Task<ApiResponse<object>> OnboardDocumentsAsync(string userId, OnboardingDocumentsRequest documentsRequest);
    Task<ApiResponse<object>> OnboardVerifyAsync(string userId, OnboardingVerifyRequest verifyRequest);
    Task<ApiResponse<UserProfileResponseDto>> UpdateProfileAsync(string userId, UpdateProfileRequestDto request);
    Task<AuthResponseDto> GoogleLoginAsync(string idToken);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<UserProfileResponseDto>> GetProfileAsync(string userId);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> LogoutAsync(string userId);
}
