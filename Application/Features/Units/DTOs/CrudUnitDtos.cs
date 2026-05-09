using Microsoft.AspNetCore.Http;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Units.DTOs;

public class CreateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int CityId { get; set; }
    public UnitType UnitType { get; set; }
    
    public List<int>? AmenityIds { get; set; }
    public List<IFormFile>? Images { get; set; }
}

public class UpdateUnitDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? Address { get; set; }
    public int? Capacity { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? CityId { get; set; }
    public UnitType? UnitType { get; set; }
    public UnitStatus? Status { get; set; }
    
    public List<int>? AmenityIds { get; set; }
    public List<IFormFile>? NewImages { get; set; }
}
