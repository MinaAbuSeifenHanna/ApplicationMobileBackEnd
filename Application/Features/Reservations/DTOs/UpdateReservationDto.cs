namespace StayHub.Backend.Application.Features.Reservations.DTOs;

public class UpdateReservationDto
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? Notes { get; set; }
}
