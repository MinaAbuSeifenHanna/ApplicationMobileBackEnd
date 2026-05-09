namespace StayHub.Backend.Application.Features.Units.DTOs;

public class UnitListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    
    public string HostName { get; set; } = string.Empty;
    public double HostRating { get; set; }
    public int ReviewsCount { get; set; }
    
    public List<string> ImagesUrl { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
}
