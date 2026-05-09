using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Amenities.DTOs;

public class UpdateAmenityDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? IconName { get; set; }
}
