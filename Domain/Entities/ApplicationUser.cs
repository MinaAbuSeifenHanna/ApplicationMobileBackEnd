using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FaceIdImageUrl { get; set; }

    [MaxLength(500)]
    public string? BackIdImageUrl { get; set; }

    public bool Owner { get; set; }

    public Gender? Gender { get; set; }

    [MaxLength(100)]
    public string? Nationality { get; set; }

    public UserRoleType RoleType { get; set; } = UserRoleType.Client;

    [MaxLength(10)]
    public string? Currency { get; set; }

    [MaxLength(50)]
    public string? BankIban { get; set; }

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }

    [MaxLength(500)]
    public string? ContractImageUrl { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    [NotMapped]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [NotMapped]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new HashSet<Unit>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public virtual ICollection<Report> ReportsSubmitted { get; set; } = new HashSet<Report>();
    public virtual ICollection<Report> ReportsAgainstMe { get; set; } = new HashSet<Report>();
    public virtual ICollection<AdminAction> AdminActionsPerformed { get; set; } = new HashSet<AdminAction>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    public virtual ICollection<ClientRequest> ClientRequests { get; set; } = new HashSet<ClientRequest>();
    public virtual ICollection<Finance> Finances { get; set; } = new HashSet<Finance>();
}
