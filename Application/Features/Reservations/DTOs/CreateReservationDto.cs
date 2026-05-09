using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Application.Features.Reservations.DTOs;

public class CreateReservationDto
{
    [Required]
    public Guid UnitId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    public string PaymentDetails { get; set; } = string.Empty; // Simulated payment info
}
