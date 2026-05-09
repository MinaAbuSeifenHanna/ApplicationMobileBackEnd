using Microsoft.AspNetCore.Http;
using StayHub.Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class OnboardingProfileRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty; // Should include country code

    [Required]
    public Gender Gender { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
