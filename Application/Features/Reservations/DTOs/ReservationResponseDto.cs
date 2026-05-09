using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Reservations.DTOs;

public class ReservationResponseDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string UnitTitle { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public UnitState State { get; set; }

    // Financial Breakdown
    public decimal BasePrice { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal Taxes { get; set; }
    public decimal TotalPrice { get; set; }

    // Payment Info
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
