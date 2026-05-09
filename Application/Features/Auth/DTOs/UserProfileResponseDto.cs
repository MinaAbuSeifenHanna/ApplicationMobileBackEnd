using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class UserProfileResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public string? Nationality { get; set; }
    public UserRoleType RoleType { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? IdFrontImageUrl { get; set; }
    public string? IdBackImageUrl { get; set; }
    public string? SelfieImageUrl { get; set; }
    public string? UnitContractDocumentUrl { get; set; }
    public string? BankAccountIban { get; set; }
    public bool IsIdentityVerified { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}
