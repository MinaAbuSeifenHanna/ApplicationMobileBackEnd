using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class Wishlist
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int UnitId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Unit Unit { get; set; } = null!;
}
