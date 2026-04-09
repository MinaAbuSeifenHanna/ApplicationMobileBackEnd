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

namespace StayHub.Backend.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IFileService _fileService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IFileService fileService,
        IValidator<RegisterDto> registerValidator,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _fileService = fileService;
        _registerValidator = registerValidator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Validation
        var validationResult = await _registerValidator.ValidateAsync(registerDto);
        if (!validationResult.IsValid)
        {
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "Validation failed", 
                Errors = validationResult.Errors.Select(e => e.ErrorMessage) 
            };
        }

        var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
        if (userExists != null)
            return new AuthResponseDto { Success = false, Message = "User already exists!" };

        // Handle File Uploads
        string faceIdUrl = await _fileService.UploadFileAsync(registerDto.FaceIdImage, "users/face-ids");
        string backIdUrl = await _fileService.UploadFileAsync(registerDto.BackIdImage, "users/back-ids");
        string profilePicUrl = await _fileService.UploadFileAsync(registerDto.ProfileImage, "users/profiles");
        string contractUrl = await _fileService.UploadFileAsync(registerDto.ContractImage, "users/contracts");

        var user = new ApplicationUser
        {
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            RoleType = registerDto.RoleType,
            PhoneNumber = registerDto.PhoneNumber,
            Gender = registerDto.Gender,
            Nationality = registerDto.Nationality,
            BankIban = registerDto.RoleType == UserRoleType.Host ? registerDto.BankIban : null,
            FaceIdImageUrl = faceIdUrl,
            BackIdImageUrl = backIdUrl,
            ProfilePictureUrl = profilePicUrl,
            ContractImageUrl = registerDto.RoleType == UserRoleType.Client ? contractUrl : null,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            // Cleanup uploaded files on failure
            _fileService.DeleteFile(faceIdUrl);
            _fileService.DeleteFile(backIdUrl);
            _fileService.DeleteFile(profilePicUrl);
            _fileService.DeleteFile(contractUrl);

            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "User creation failed!", 
                Errors = result.Errors.Select(x => x.Description) 
            };
        }

        // Assign Role
        var roleName = registerDto.RoleType.ToString();
        await EnsureRoleExistsAsync(roleName);
        await _userManager.AddToRoleAsync(user, roleName);

        return new AuthResponseDto { Success = true, Message = "User registered successfully!" };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return new AuthResponseDto { Success = false, Message = "Invalid credentials!" };

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

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
            Message = "Login successful!"
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        if (principal == null)
            return new AuthResponseDto { Success = false, Message = "Invalid access token or refresh token" };

        string username = principal.Identity!.Name!;
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new AuthResponseDto { Success = false, Message = "Invalid access token or refresh token" };

        var newAccessToken = CreateToken(principal.Claims.ToList());
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            Message = "Token refreshed successfully!"
        };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return true;
    }

    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.Now.AddDays(_jwtSettings.ExpiryInDays),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
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
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
