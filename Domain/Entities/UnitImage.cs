using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class UnitImage
{
    public int Id { get; set; }

    public Guid UnitId { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public virtual Unit Unit { get; set; } = null!;
}
