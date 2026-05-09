using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Auth.DTOs;

public class OnboardingDocumentsRequest
{
    [Required]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    public string BankAccountIban { get; set; } = string.Empty;

    public IFormFile? IdFrontImage { get; set; }
    public IFormFile? IdBackImage { get; set; }
    public IFormFile? UnitContractDocument { get; set; }
}
