using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Offers.DTOs;

public class SearchRequestDto
{
    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string PropertyType { get; set; } = string.Empty;

    [Required]
    public DateTime MoveInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 100)]
    public int GuestNumber { get; set; }

    [Range(0, 1000000)]
    public decimal MaxBudget { get; set; }

    public string? AdditionalNotes { get; set; }
}
