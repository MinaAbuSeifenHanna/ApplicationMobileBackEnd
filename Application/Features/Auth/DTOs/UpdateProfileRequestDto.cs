using Microsoft.AspNetCore.Http;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class UpdateProfileRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfileImage { get; set; }
}
