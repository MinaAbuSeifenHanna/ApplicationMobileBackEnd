using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class Location
{
    public int Id { get; set; }

    [Required]
    public int UnitId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(500)]
    public string? GoogleMapUrl { get; set; }

    [MaxLength(200)]
    public string? PlaceId { get; set; }

    // Navigation Property
    public virtual Unit Unit { get; set; } = null!;
}
