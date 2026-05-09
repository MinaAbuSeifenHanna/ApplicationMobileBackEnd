using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class UnitReservation
{
    public Guid Id { get; set; }

    public Guid UnitId { get; set; }

    [Required]
    public string GuestId { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public UnitState State { get; set; }

    // Financial Details
    [Required]
    public string BookingReference { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public decimal ServiceFee { get; set; }

    public decimal Taxes { get; set; }

    public decimal TotalPrice { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Unit Unit { get; set; } = null!;
    public virtual ApplicationUser Guest { get; set; } = null!;
}
