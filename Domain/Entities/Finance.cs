using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Finance
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public FinanceType Type { get; set; }

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual ApplicationUser User { get; set; } = null!;
}
