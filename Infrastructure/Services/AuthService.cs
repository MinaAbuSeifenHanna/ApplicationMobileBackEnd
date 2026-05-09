using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StayHub.Backend.Application.Features.Auth.DTOs;
using StayHub.Backend.Application.Features.Auth.Interfaces;
using StayHub.Backend.Domain.Entities;
using StayHub.Backend.Domain.Enums;
using StayHub.Backend.Infrastructure.Security;
using StayHub.Backend.Application.Common.Interfaces;
using Google.Apis.Auth;

namespace StayHub.Backend.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IFileService _fileService;
    private readonly IPhotoService _photoService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<OnboardingProfileRequest> _profileValidator;
    private readonly IValidator<OnboardingDocumentsRequest> _documentsValidator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IFileService fileService,
        IPhotoService photoService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<OnboardingProfileRequest> profileValidator,
        IValidator<OnboardingDocumentsRequest> documentsValidator,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _fileService = fileService;
        _photoService = photoService;
        _registerValidator = registerValidator;
        _profileValidator = profileValidator;
        _documentsValidator = documentsValidator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerRequest);
        if (!validationResult.IsValid)
            return new AuthResponseDto { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage) };

        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (userExists != null)
            return new AuthResponseDto { Success = false, Message = "Email already registered!" };

        var usernameExists = await _userManager.FindByNameAsync(registerRequest.Username);
        if (usernameExists != null)
            return new AuthResponseDto { Success = false, Message = "Username is already taken!" };

        var user = new ApplicationUser
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Username,
            RoleType = registerRequest.Role,
            CreatedAt = DateTime.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
            return new AuthResponseDto { Success = false, Message = "User creation failed!", Errors = result.Errors.Select(x => x.Description) };

        var roleName = registerRequest.Role.ToString();
        await EnsureRoleExistsAsync(roleName);
        await _userManager.AddToRoleAsync(user, roleName);

        return await LoginAsync(new LoginDto { Email = user.Email, Password = registerRequest.Password });
    }

    public async Task<ApiResponse<object>> OnboardProfileAsync(string userId, OnboardingProfileRequest profileRequest)
    {
        var validationResult = await _profileValidator.ValidateAsync(profileRequest);
        if (!validationResult.IsValid)
            return ApiResponse<object>.FailureResult("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return ApiResponse<object>.FailureResult("User not found");

        user.FirstName = profileRequest.FirstName;
        user.LastName = profileRequest.LastName;
        user.PhoneNumber = profileRequest.PhoneNumber;
        user.Gender = profileRequest.Gender;

        string? profileImageUrl = null;
        if (profileRequest.ProfileImage != null)
        {
            profileImageUrl = await _photoService.UploadImageAsync(profileRequest.ProfileImage);
            user.ProfileImageUrl = profileImageUrl;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse<object>.FailureResult("Failed to update profile", result.Errors.Select(e => e.Description));

        return ApiResponse<object>.SuccessResult(new { ProfileImageUrl = user.ProfileImageUrl }, "Profile updated successfully");
    }

    public async Task<ApiResponse<object>> OnboardDocumentsAsync(string userId, OnboardingDocumentsRequest documentsRequest)
    {
        var validationResult = await _documentsValidator.ValidateAsync(documentsRequest);
        if (!validationResult.IsValid)
            return ApiResponse<object>.FailureResult("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return ApiResponse<object>.FailureResult("User not found");

        user.Nationality = documentsRequest.Nationality;
        user.BankAccountIban = documentsRequest.BankAccountIban;

        if (documentsRequest.IdFrontImage != null)
            user.IdFrontImageUrl = await _photoService.UploadImageAsync(documentsRequest.IdFrontImage);
        
        if (documentsRequest.IdBackImage != null)
            user.IdBackImageUrl = await _photoService.UploadImageAsync(documentsRequest.IdBackImage);

        if (documentsRequest.UnitContractDocument != null)
        {
            user.UnitContractDocumentUrl = await _fileService.UploadFileAsync(documentsRequest.UnitContractDocument, "contracts");
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse<object>.FailureResult("Failed to update documents", result.Errors.Select(e => e.Description));

        return ApiResponse<object>.SuccessResult(new 
        { 
            user.IdFrontImageUrl, 
            user.IdBackImageUrl, 
            user.UnitContractDocumentUrl 
        }, "Documents uploaded successfully");
    }

    public async Task<ApiResponse<object>> OnboardVerifyAsync(string userId, OnboardingVerifyRequest verifyRequest)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return ApiResponse<object>.FailureResult("User not found");

        if (verifyRequest.SelfieImage != null)
        {
            user.SelfieImageUrl = await _photoService.UploadImageAsync(verifyRequest.SelfieImage);
            user.IsIdentityVerified = true; 
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse<object>.FailureResult("Failed to submit verification", result.Errors.Select(e => e.Description));

        return ApiResponse<object>.SuccessResult(new { user.SelfieImageUrl }, "Identity verification submitted");
    }

    public async Task<ApiResponse<UserProfileResponseDto>> UpdateProfileAsync(string userId, UpdateProfileRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserProfileResponseDto>.FailureResult("User not found");

        // Partial update logic: only update if value is provided
        if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
        if (request.Gender.HasValue) user.Gender = request.Gender.Value;
        if (request.Bio != null) user.Bio = request.Bio; // Bio can be cleared if explicitly set to empty string

        // Only update ProfileImageUrl if a new file is uploaded
        if (request.ProfileImage != null && request.ProfileImage.Length > 0)
        {
            user.ProfileImageUrl = await _photoService.UploadImageAsync(request.ProfileImage);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse<UserProfileResponseDto>.FailureResult("Failed to update profile", result.Errors.Select(e => e.Description));

        // Reuse GetProfileAsync logic to return the updated DTO
        return await GetProfileAsync(userId);
    }

    public async Task<ApiResponse<UserProfileResponseDto>> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserProfileResponseDto>.FailureResult("User not found");

        var profileDto = new UserProfileResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Gender,
            Nationality = user.Nationality,
            RoleType = user.RoleType,
            ProfileImageUrl = user.ProfileImageUrl,
            IdFrontImageUrl = user.IdFrontImageUrl,
            IdBackImageUrl = user.IdBackImageUrl,
            SelfieImageUrl = user.SelfieImageUrl,
            UnitContractDocumentUrl = user.UnitContractDocumentUrl,
            BankAccountIban = user.BankAccountIban,
            IsIdentityVerified = user.IsIdentityVerified,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt
        };

        return ApiResponse<UserProfileResponseDto>.SuccessResult(profileDto, "Profile retrieved successfully");
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return new AuthResponseDto { Success = false, Message = "Invalid credentials!" };

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    ProfileImageUrl = payload.Picture,
                    EmailConfirmed = true,
                    RoleType = UserRoleType.Client, // Default role for Google login
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return new AuthResponseDto { Success = false, Message = "Google user creation failed" };

                await EnsureRoleExistsAsync("Client");
                await _userManager.AddToRoleAsync(user, "Client");
            }

            return await GenerateAuthResponse(user);
        }
        catch (InvalidJwtException)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid Google token" };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        if (principal == null)
            return new AuthResponseDto { Success = false, Message = "Invalid token" };

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new AuthResponseDto { Success = false, Message = "Invalid refresh token" };

        return await GenerateAuthResponse(user);
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return true;
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("role", userRoles.FirstOrDefault() ?? "Client")
        };

        foreach (var role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = CreateToken(authClaims);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            UserId = user.Id,
            Email = user.Email,
            Role = userRoles.FirstOrDefault(),
            Message = "Operation successful"
        };
    }

    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        return new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryInDays),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));
    }
}
