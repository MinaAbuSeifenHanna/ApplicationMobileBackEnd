using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class Report
{
    public int Id { get; set; }

    [Required]
    public string ReporterId { get; set; } = string.Empty;

    public string? TargetUserId { get; set; }

    public Guid? UnitId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ApplicationUser Reporter { get; set; } = null!;
    public virtual ApplicationUser? TargetUser { get; set; }
    public virtual Unit? Unit { get; set; }
    public virtual ICollection<AdminAction> AdminActions { get; set; } = new HashSet<AdminAction>();
}
