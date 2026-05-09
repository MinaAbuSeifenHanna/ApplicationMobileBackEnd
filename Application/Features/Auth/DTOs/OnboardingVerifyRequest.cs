using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class OnboardingVerifyRequest
{
    [Required]
    public IFormFile SelfieImage { get; set; } = null!;
}
