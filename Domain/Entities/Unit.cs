using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Unit
{
    public int Id { get; set; }

    [Required]
    public string OwnerId { get; set; } = string.Empty;

    public int CityId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public decimal PricePerNight { get; set; }

    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [Range(1, 50)]
    public int Capacity { get; set; }

    public UnitType UnitType { get; set; }

    public UnitStatus Status { get; set; } = UnitStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? Filter { get; set; }

    public virtual ApplicationUser OwnerUser { get; set; } = null!;
    public virtual ICollection<UnitImage> Images { get; set; } = new HashSet<UnitImage>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    public virtual ICollection<UnitAmenity> UnitAmenities { get; set; } = new HashSet<UnitAmenity>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    public virtual Location? Location { get; set; }
    public virtual ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();
}
