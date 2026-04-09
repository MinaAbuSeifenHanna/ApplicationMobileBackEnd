using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; } = false;

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [MaxLength(50)]
    public string? ReferenceType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual ApplicationUser User { get; set; } = null!;
}
