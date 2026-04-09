using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int ReservationId { get; set; }

    public decimal Amount { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public PaymentProvider PaymentProvider { get; set; }

    [MaxLength(200)]
    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Reservation Reservation { get; set; } = null!;
}
