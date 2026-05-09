using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }

    public Guid UnitId { get; set; }

    [Required]
    public string GuestId { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    [NotMapped]
    public int Period => (CheckOutDate.Date - CheckInDate.Date).Days;

    public decimal TotalPrice { get; set; }
    public decimal Deposit { get; set; }

    [NotMapped]
    public decimal AfterDeposit => TotalPrice - Deposit;

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public virtual Unit Unit { get; set; } = null!;
    public virtual ApplicationUser Guest { get; set; } = null!;
    public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}
