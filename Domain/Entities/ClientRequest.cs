using System.ComponentModel.DataAnnotations;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class ClientRequest
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public decimal MinPrice { get; set; }

    public decimal MaxPrice { get; set; }

    public int GuestsCount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public ClientRequestStatus Status { get; set; } = ClientRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual ApplicationUser User { get; set; } = null!;
}
