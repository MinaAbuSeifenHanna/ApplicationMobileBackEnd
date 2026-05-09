using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class Offer
{
    public int Id { get; set; }

    [Required]
    public Guid UnitId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual Unit Unit { get; set; } = null!;
}
