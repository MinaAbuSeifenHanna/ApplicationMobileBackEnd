using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class Review
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Reservation Reservation { get; set; } = null!;
}
