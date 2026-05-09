namespace StayHub.Backend.Application.Features.Offers.DTOs;

public class OfferResponseDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Amenities { get; set; } = new();
}
