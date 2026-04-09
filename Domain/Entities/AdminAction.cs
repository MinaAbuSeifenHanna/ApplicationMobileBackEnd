using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class AdminAction
{
    public int Id { get; set; }

    [Required]
    public int ReportId { get; set; }

    [Required]
    public string AdminId { get; set; } = string.Empty;

    public AdminActionType Action { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Report Report { get; set; } = null!;
    public virtual ApplicationUser AdminUser { get; set; } = null!;
}
