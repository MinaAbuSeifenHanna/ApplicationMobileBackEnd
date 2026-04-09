using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public UserRoleType RoleType { get; set; } = UserRoleType.Client;

    public Gender? Gender { get; set; }

    public string? Nationality { get; set; }

    // Images
    public IFormFile? FaceIdImage { get; set; }
    public IFormFile? BackIdImage { get; set; }
    public IFormFile? ProfileImage { get; set; }

    // Host-specific
    public string? BankIban { get; set; }

    // Client-specific
    public IFormFile? ContractImage { get; set; }
}
