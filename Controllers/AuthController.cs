using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayHub.Backend.Application.Features.Auth.DTOs;
using StayHub.Backend.Application.Features.Auth.Interfaces;
using System.Security.Claims;

namespace StayHub.Backend.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.GetProfileAsync(userId);
        if (!result.Success) return NotFound(result);

        return Ok(result);
    }

    [HttpPut("profile")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequestDto profileRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.UpdateProfileAsync(userId, profileRequest);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("onboarding/profile")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> OnboardProfile([FromForm] OnboardingProfileRequest profileRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.OnboardProfileAsync(userId, profileRequest);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("onboarding/documents")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> OnboardDocuments([FromForm] OnboardingDocumentsRequest documentsRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.OnboardDocumentsAsync(userId, documentsRequest);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("onboarding/verify")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> OnboardVerify([FromForm] OnboardingVerifyRequest verifyRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.OnboardVerifyAsync(userId, verifyRequest);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var result = await _authService.GoogleLoginAsync(request.IdToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        if (tokenRequest is null)
            return BadRequest("Invalid client request");

        var result = await _authService.RefreshTokenAsync(tokenRequest.Token, tokenRequest.RefreshToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return BadRequest("User not found");

        var result = await _authService.LogoutAsync(userId);
        if (!result) return BadRequest("Logout failed");

        return Ok(new { Message = "Logged out successfully" });
    }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class TokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
